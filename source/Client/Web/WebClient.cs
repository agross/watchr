using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

using Client.Messages;

using Microsoft.AspNet.SignalR.Client.Hubs;

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

  public class WebClient : IDisposable
  {
    readonly HubConnection _connection;
    readonly IHubProxy _hub;
    readonly CompositeDisposable _subscriptions;

    public WebClient(string url)
    {
      Console.WriteLine("Starting web client for {0}", url);

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
            Console.WriteLine("Buffering until connection becomes available");
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
      Console.WriteLine("{0} SignalR: Starting connection", Thread.CurrentThread.ManagedThreadId);

      try
      {
        await connection.Start().ConfigureAwait(false);
      }
      catch (Exception exception)
      {
        Console.WriteLine("{0} SignalR: Could not start connection: {1}",
                          Thread.CurrentThread.ManagedThreadId,
                          exception);
        throw;
      }
    }

    async void Send(BlockParsed output)
    {
      Console.WriteLine("{0} Session {1}: Sending {2} lines, starting at index {3}",
                        Thread.CurrentThread.ManagedThreadId,
                        output.SessionId,
                        output.Lines.Count(),
                        output.Lines.First().Index);

      try
      {
        await _hub.Invoke<string>("Broadcast", output).ConfigureAwait(false);
      }
      catch (Exception exception)
      {
        Console.WriteLine("{0} Session {1}: Error sending request {2}",
                          Thread.CurrentThread.ManagedThreadId,
                          output.SessionId,
                          exception);
        throw;
      }
    }

    async void Terminate(SessionTerminated message)
    {
      Console.WriteLine("{0} Session {1}: Closing session",
                        Thread.CurrentThread.ManagedThreadId,
                        message.SessionId);

      try
      {
        await _hub.Invoke<string>("Terminate", message.SessionId).ConfigureAwait(false);
      }
      catch (Exception exception)
      {
        Console.WriteLine("{0} Session {1}: Error sending request {2}",
                          Thread.CurrentThread.ManagedThreadId,
                          message.SessionId,
                          exception);
        throw;
      }
    }
  }
}
