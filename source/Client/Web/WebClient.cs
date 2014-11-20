using System;
using System.Configuration;
using System.Linq;

using Client.Messages;
using Client.Minimods;

using Microsoft.AspNet.SignalR.Client.Hubs;

namespace Client.Web
{
  public class WebClient : IDisposable
  {
    readonly HubConnection _connection;
    readonly IHubProxy _hub;
    readonly IDisposable[] _subscription;

    public WebClient()
    {
      Console.WriteLine("Starting web client");

      _connection = new HubConnection(ConfigurationManager.AppSettings["base-address"]);
      _hub = _connection.CreateHubProxy("ConsoleHub");

      _connection.Start().ContinueWith(task =>
      {
        if (task.IsFaulted)
        {
          Console.WriteLine("There was an error opening the connection: {0}",
                                   task.Exception.GetBaseException());
        }
        else
        {
          Console.WriteLine("Connected");
        }
      }).Wait();

      _subscription = new[]
                      {
                        RxMessageBrokerMinimod.Default.Register<BlockParsed>(Send),
                        RxMessageBrokerMinimod.Default.Register<SessionTerminated>(Terminate)
                      };
    }

    public void Dispose()
    {
      Array.ForEach(_subscription, x => x.Dispose());
      _connection.Stop();
    }

    async void Terminate(SessionTerminated message)
    {
      Console.WriteLine("Session {0}: Closing session", message.SessionId);

      try
      {
        await _hub.Invoke<string>("Terminate", message.SessionId);
      }
      catch (Exception exception)
      {
        Console.WriteLine("Session {0}: Error sending request {1}", message.SessionId, exception);
      }
    }

    async void Send(BlockParsed output)
    {
      Console.WriteLine("Session {0}: Sending {1} lines, starting at index {2}",
                               output.SessionId,
                               output.Lines.Count(),
                               output.Lines.First().Index);

      try
      {
        await _hub.Invoke<string>("Broadcast", output);
      }
      catch (Exception exception)
      {
        Console.WriteLine("Session {0}: Error sending request {1}", output.SessionId, exception);
      }
    }
  }
}
