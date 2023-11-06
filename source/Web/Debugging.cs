using Client.Messages;

using Microsoft.AspNetCore.SignalR;

using Web.Hubs;

namespace Web;

public class Debugging : BackgroundService
{
  readonly IHubContext<ShellHub> _hub;

  public Debugging(IHubContext<ShellHub> hub)
  {
    _hub = hub;
  }

  protected override Task ExecuteAsync(CancellationToken cancellationToken)
    => Task.Run(async () =>
                {
                  while (!cancellationToken.IsCancellationRequested)
                  {
                    Console.WriteLine("Enter something to send");
                    var text = Console.ReadLine();

                    await _hub.Clients.All.SendAsync("text",
                                                     new TextReceived("Debug Session",
                                                                      0,
                                                                      text.Length,
                                                                      text),
                                                     cancellationToken);
                  }
                },
                cancellationToken);
}
