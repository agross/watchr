using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Web
{
  public class ConsoleHub : Hub
  {
    static readonly string globalGroupId = "8bf6b1c0-34c5-4921-a712-8c4a3564c8b8";

    public void Broadcast(string groupId, object text)
    {
      GroupForInstance(groupId).text(text);
    }

    public void Terminate(string groupId, string sessionId)
    {
      GroupForInstance(groupId).terminate(sessionId);
    }

    public Task JoinGroup(string searchQuery)
    {
      if (string.IsNullOrWhiteSpace(searchQuery))
      {
        return Groups.Add(Context.ConnectionId, globalGroupId);
      }

      return Groups.Add(Context.ConnectionId, searchQuery.Remove(0, 1));
    }

    dynamic GroupForInstance(string groupId)
    {
      if(string.IsNullOrWhiteSpace(groupId))
      {
        return Clients.Group(globalGroupId);
      }

      return Clients.Group(groupId);
    }
  }
}
