using Microsoft.AspNet.SignalR;

namespace Web
{
  public class ConsoleHub : Hub
  {
    public void Broadcast(object text)
    {
      GlobalHost.ConnectionManager.GetHubContext<ConsoleHub>().Clients.All.text(text);
    }

    public void Terminate(string sessionId)
    {
      GlobalHost.ConnectionManager.GetHubContext<ConsoleHub>().Clients.All.terminate(sessionId);
    }
  }
}
