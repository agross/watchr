using System.Collections.Concurrent;
using System.Text.RegularExpressions;

using Client.Messages;

using Microsoft.AspNetCore.SignalR;

using Web.Hubs;

namespace Web;

public partial class Debugging : BackgroundService
{
  record Offsets(int Start, int End, int HoleStartsAt = 0);

  readonly IHubContext<ShellHub> _hub;

  [GeneratedRegex("^(\\w+)\\s+(.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
  private static partial Regex LineRegex();

  static readonly Regex Line = LineRegex();
  readonly ConcurrentDictionary<string, Offsets> _sessionOffsets = new();

  public Debugging(IHubContext<ShellHub> hub)
  {
    _hub = hub;
  }

  protected override Task ExecuteAsync(CancellationToken cancellationToken)
    => Task.Run(() => Menu(cancellationToken), cancellationToken);

  async Task Menu(CancellationToken cancellationToken)
  {
    while (!cancellationToken.IsCancellationRequested)
    {
      Console.WriteLine("""
                        Enter something to send: <Session ID> <Selection>
                        1: "hello"
                        2: a very long string
                        3: a link
                        4: make session miss an update
                        5: make session catch up
                        6: 2 sessions with 100 lines (session ID is ignored)
                        otherwise: send string
                        """);
      var line = Console.ReadLine();

      var match = Line.Match(line ?? "");
      if (!match.Success)
      {
        Console.WriteLine("Huh?");

        continue;
      }

      var sessionId = match.Groups[1].Value;
      var selection = match.Groups[2].Value;

      var action = selection switch
      {
        "1" => SendText("normal", sessionId, "hello\r\n", cancellationToken),
        "2" => SendText("normal", sessionId, new string('m', 100) + "\r\n", cancellationToken),
        "3" => SendText("normal", sessionId, "https://example.com/\r\n", cancellationToken),
        "4" => SendText("early", sessionId, "early text\r\n", cancellationToken),
        "5" => SendText("late", sessionId, LateText, cancellationToken),
        "6" => TwoSessions(cancellationToken),
        _ => SendText("normal", sessionId, selection + "\r\n", cancellationToken),
      };

      await action();
    }
  }

  Func<Task> TwoSessions(CancellationToken cancellationToken)
  {
    var rnd = new Random();

    return async () => await Parallel
      .ForEachAsync(new[] { "Session 1", "Session 2" },
                    cancellationToken,
                    async (sessionId, ct) =>
                    {
                      foreach (var i in Enumerable.Range(1, 100))
                      {
                        await Task.Delay(TimeSpan.FromMilliseconds(100) * rnd.Next(10), ct);

                        await SendText("normal",
                                       sessionId,
                                       $"{sessionId}, iteration {i}\r\n",
                                       ct)();
                      }
                    });
  }

  Func<Task> SendText(string offsetHandling,
                      string sessionId,
                      string text,
                      CancellationToken cancellationToken)
  {
    return async () =>
    {
      var (startOffset, endOffset, _) = offsetHandling switch
      {
        "early" => MakeEarly(_sessionOffsets, sessionId, text),
        "late" => MakeLate(_sessionOffsets, sessionId, text),
        _ => AdvanceOffsetNormally(_sessionOffsets, sessionId, text),
      };

      await _hub.Clients.All.SendAsync("text",
                                       new TextReceived(sessionId,
                                                        startOffset,
                                                        endOffset,
                                                        text),
                                       cancellationToken);
    };
  }

  const string LateText = "late text\r\n";

  static Offsets AdvanceOffsetNormally(ConcurrentDictionary<string, Offsets> sessionOffsets,
                                       string sessionId,
                                       string text)
  {
    if (!sessionOffsets.TryGetValue(sessionId, out var offset))
    {
      offset = new Offsets(0, 0);
    }

    sessionOffsets[sessionId] = new Offsets(offset.End,
                                            offset.End + text.Length);

    return sessionOffsets[sessionId];
  }

  static Offsets MakeEarly(ConcurrentDictionary<string, Offsets> sessionOffsets,
                           string sessionId,
                           string text)
  {
    if (!sessionOffsets.TryGetValue(sessionId, out var offset))
    {
      offset = new Offsets(0, 0);
    }

    sessionOffsets[sessionId] = new Offsets(offset.End + LateText.Length,
                                            offset.End + LateText.Length + text.Length,
                                            offset.End);

    return sessionOffsets[sessionId];
  }

  static Offsets MakeLate(ConcurrentDictionary<string, Offsets> sessionOffsets,
                          string sessionId,
                          string text)
  {
    if (!sessionOffsets.TryGetValue(sessionId, out var offset))
    {
      offset = new Offsets(0, 0);
    }

    return new Offsets(offset.HoleStartsAt,
                       offset.HoleStartsAt + text.Length);
  }
}
