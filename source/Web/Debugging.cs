using System.Text.RegularExpressions;

using Client.Messages;

using Microsoft.AspNetCore.SignalR;

using Web.Hubs;

namespace Web;

public class Debugging : BackgroundService
{
  readonly IHubContext<ShellHub> _hub;

  static readonly Regex Line = new(@"^(\w+)\s+(.*)",
                                   RegexOptions.Compiled | RegexOptions.CultureInvariant);

  public Debugging(IHubContext<ShellHub> hub)
  {
    _hub = hub;
  }

  protected override Task ExecuteAsync(CancellationToken cancellationToken)
    => Task.Run(async () =>
                {
                  var sessionOffsets = new Dictionary<string, int>();

                  while (!cancellationToken.IsCancellationRequested)
                  {
                    Console.WriteLine("Enter something to send: <Session> <Text>");
                    var line = Console.ReadLine();

                    if (!Line.IsMatch(line ?? ""))
                    {
                      Console.WriteLine("Huh?");
                      continue;
                    }

                    var sessionId = Line.Match(line!).Groups[1].Value;
                    var text = Line.Match(line).Groups[2].Value;
                    text += "\r\n";

                    var startOffset = 0;
                    if (sessionOffsets.TryGetValue(sessionId, out var offset))
                    {
                      startOffset = offset;
                    }

                    sessionOffsets[sessionId] = startOffset + text.Length;

                    await _hub.Clients.All.SendAsync("text",
                                                     new TextReceived(sessionId,
                                                                      startOffset,
                                                                      sessionOffsets[sessionId],
                                                                      text),
                                                     cancellationToken);
                  }
                },
                cancellationToken);
}
