using Microsoft.AspNet.SignalR;

namespace Web
{
  public class ConsoleHub : Hub
  {
    public void Broadcast(object block)
    {
      GlobalHost.ConnectionManager.GetHubContext<ConsoleHub>().Clients.All.block(block);
    }
    
    public void Terminate(string sessionId)
    {
      GlobalHost.ConnectionManager.GetHubContext<ConsoleHub>().Clients.All.terminate(sessionId);
    }
  }
}
