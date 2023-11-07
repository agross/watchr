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

  public Debugging(IHubContext<ShellHub> hub)
  {
    _hub = hub;
  }

  protected override Task ExecuteAsync(CancellationToken cancellationToken)
    => Task.Run(() => Menu(cancellationToken), cancellationToken);

  async Task Menu(CancellationToken cancellationToken)
  {
    var sessionOffsets = new Dictionary<string, Offsets>();

    while (!cancellationToken.IsCancellationRequested)
    {
      Console.WriteLine("""
                        Enter something to send: <Session ID> <Selection>
                        1: "hello"
                        2: a very long string
                        3: a link
                        4: make session miss an update
                        5: make session catch up
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
      var (text, offsetHandling) = selection switch
      {
        "1" => ("hello\r\n", "normal"),
        "2" => (new string('m', 100) + "\r\n", "normal"),
        "3" => ("https://example.com/\r\n", "normal"),
        "4" => ("early text\r\n", "early"),
        "5" => (LateText, "late"),
        _ => (selection + "\r\n", "normal"),
      };

      var (startOffset, endOffset, _) = offsetHandling switch
      {
        "early" => MakeEarly(sessionOffsets, sessionId, text),
        "late" => MakeLate(sessionOffsets, sessionId, text),
        _ => AdvanceOffsetNormally(sessionOffsets, sessionId, text),
      };

      await _hub.Clients.All.SendAsync("text",
                                       new TextReceived(sessionId,
                                                        startOffset,
                                                        endOffset,
                                                        text),
                                       cancellationToken);
    }
  }

  const string LateText = "late text\r\n";

  static Offsets AdvanceOffsetNormally(Dictionary<string, Offsets> sessionOffsets,
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

  static Offsets MakeEarly(Dictionary<string, Offsets> sessionOffsets,
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

  static Offsets MakeLate(Dictionary<string, Offsets> sessionOffsets,
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
