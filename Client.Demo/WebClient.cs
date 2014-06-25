using System;
using System.Linq;

using Client.Console.Messages;

using Microsoft.AspNet.SignalR.Client.Hubs;

using Minimod.RxMessageBroker;

namespace Client.Demo
{
  class WebClient : IDisposable
  {
    const string BaseAddress = "http://localhost:34530/";
    readonly HubConnection _connection;
    readonly IHubProxy _hub;
    readonly IDisposable[] _subscription;

    public WebClient()
    {
      System.Console.WriteLine("Starting web client");

      _connection = new HubConnection(BaseAddress);
      _hub = _connection.CreateHubProxy("ConsoleHub");

      _connection.Start().ContinueWith(task =>
      {
        if (task.IsFaulted)
        {
          System.Console.WriteLine("There was an error opening the connection:{0}",
                                   task.Exception.GetBaseException());
        }
        else
        {
          System.Console.WriteLine("Connected");
        }
      }).Wait();

      _subscription = new[]
                      {
                        RxMessageBrokerMinimod.Default.Register<BlockParsed>(Send),
                        RxMessageBrokerMinimod.Default.Register<SessionTerminated>(Close)
                      };
    }

    public void Dispose()
    {
      Array.ForEach(_subscription, x => x.Dispose());
      _connection.Stop();
    }

    async void Close(SessionTerminated message)
    {
      System.Console.WriteLine("Session {0}: Closing session", message.SessionId);

      try
      {
        await _hub.Invoke<string>("Broadcast", message.SessionId);
      }
      catch (Exception exception)
      {
        System.Console.WriteLine("Session {0}: Error sending request {1}", message.SessionId, exception);
      }
    }

    async void Send(BlockParsed output)
    {
      System.Console.WriteLine("Session {0}: Sending {1} lines, starting at index {2}",
                               output.SessionId,
                               output.Lines.Count(),
                               output.Lines.First().Index);

      try
      {
        await _hub.Invoke<string>("Broadcast", output);
      }
      catch (Exception exception)
      {
        System.Console.WriteLine("Session {0}: Error sending request {1}", output.SessionId, exception);
      }
    }
  }
}
