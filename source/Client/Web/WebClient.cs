using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Client.Messages;

using Microsoft.AspNet.SignalR.Client.Hubs;

using NLog;

namespace Client.Web
{
  static class ClassNameExtensions
  {
    public static IObservable<T> RetryDo<T, TException>(this IObservable<T> instance,
                                                        Action<T> onNext) where TException : Exception
    {
      return instance
        .Do(x =>
        {
          var failed = true;
          while (failed)
          {
            try
            {
              onNext(x);
              failed = false;
            }
            catch (TException)
            {
            }
          }
        });
    }
  }

  class WebClient : IDisposable
  {
    static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    readonly HubConnection _connection;
    readonly IHubProxy _hub;
    readonly CompositeDisposable _subscriptions;

    public WebClient(string url)
    {
      Logger.Info("Starting web client for {0}", url);

      _connection = new HubConnection(url);
      _hub = _connection.CreateHubProxy("ConsoleHub");

      var connection = new Connection(_connection);

      var onlineMessages = new Messages()
        .Stream
        .Window(connection.State)
        .Select((d, index) =>
        {
          var buffer = new Func<int, bool>(i => i % 2 != 0);
          if (buffer(index))
          {
            Logger.Warn("Buffering until connection becomes available");
            return d.ToList().SelectMany(x => x);
          }
          return d;
        })
        .Concat()
        .Publish();

      var eventLoop = new EventLoopScheduler();

      _subscriptions = new CompositeDisposable(
        connection
          .ConnectRequired
          .ObserveOn(eventLoop)
          .Do(Connect)
          .Retry()
          .Subscribe(),
        onlineMessages
          .OfType<BlockParsed>()
          .ObserveOn(eventLoop)
          .RetryDo<BlockParsed, Exception>(Send)
          .Subscribe(),
        onlineMessages
          .OfType<SessionTerminated>()
          .ObserveOn(eventLoop)
          .RetryDo<SessionTerminated, Exception>(Terminate)
          .Subscribe(),
        onlineMessages.Connect()
        );
    }

    public void Dispose()
    {
      _subscriptions.Dispose();
      _connection.Stop();
    }

    static async void Connect(Microsoft.AspNet.SignalR.Client.Connection connection)
    {
      Logger.Info("SignalR: Starting connection");

      try
      {
        await connection.Start().ConfigureAwait(false);
      }
      catch (Exception exception)
      {
        Logger.Error("SignalR: Could not start connection", exception);
        throw;
      }
    }

    async void Send(BlockParsed output)
    {
      Logger.Info("Session {0}: Sending {1} lines, starting at index {2}",
                  output.SessionId,
                  output.Lines.Count(),
                  output.Lines.First().Index);

      try
      {
        await _hub.Invoke<string>("Broadcast", output).ConfigureAwait(false);
      }
      catch (Exception exception)
      {
        Logger.Error(String.Format("Session {0}: Error sending request",
                                   output.SessionId),
                     exception);
        throw;
      }
    }

    async void Terminate(SessionTerminated message)
    {
      Logger.Info("Session {0}: Terminated", message.SessionId);

      try
      {
        await _hub.Invoke<string>("Terminate", message.SessionId).ConfigureAwait(false);
      }
      catch (Exception exception)
      {
        Logger.Error(String.Format("Session {0}: Error sending request",
                                   message.SessionId),
                     exception);
        throw;
      }
    }
  }
}
