using Client.Messages;

using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs;

public class ShellHub : Hub
{
  internal const string DefaultGroupId = "8bf6b1c0-34c5-4921-a712-8c4a3564c8b8";

  public void Broadcast(string groupId, TextReceived text)
  {
    GroupForInstance(groupId).SendAsync("text", text);
  }

  public void Terminate(string groupId, string sessionId)
  {
    GroupForInstance(groupId).SendAsync("terminate", sessionId);
  }

  public Task JoinGroup(string searchQuery)
  {
    var groupId = DefaultGroupId;

    if (!string.IsNullOrWhiteSpace(searchQuery))
    {
      groupId = searchQuery.Remove(0, 1);
    }

    return Groups.AddToGroupAsync(Context.ConnectionId, groupId);
  }

  IClientProxy GroupForInstance(string groupId)
  {
    if (string.IsNullOrWhiteSpace(groupId))
    {
      groupId = DefaultGroupId;
    }

    return Clients.Group(groupId);
  }
}
