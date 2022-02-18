using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Client.Web;

public class Connection : IAsyncDisposable
{
  readonly ILogger<Connection> _logger;

  public string Url { get; }
  public HubConnection Hub { get; private set; }

  readonly ISubject<object> _state = new Subject<object>();
  CompositeDisposable _subscriptions;

  public Connection(ILogger<Connection> logger, HubOptions options)
  {
    _logger = logger;

    Url = options.Url;
  }

  public async Task Start(TimeSpan reconnectTimeout,
                          CancellationToken cancellationToken)
  {
    Hub = new HubConnectionBuilder()
          .WithUrl(Url)
          .WithAutomaticReconnect(new AlwaysRetryPolicy(reconnectTimeout))
          .Build();

    _subscriptions = new CompositeDisposable(Observable
                                             .FromEvent<Func<string, Task>, string>(action => str =>
                                                                                    {
                                                                                      action(str);

                                                                                      return Task.CompletedTask;
                                                                                    },
                                                                                    f => Hub.Reconnected += f,
                                                                                    f => Hub.Reconnected -= f)
                                             .Do(_ => ConnectionEstablished())
                                             .Subscribe(),
                                             Observable
                                               .FromEvent<Func<Exception, Task>, Exception>(action => ex =>
                                                                                            {
                                                                                              action(ex);

                                                                                              return Task.CompletedTask;
                                                                                            },
                                                                                            f => Hub.Reconnecting += f,
                                                                                            f => Hub.Reconnecting -= f)
                                               .Do(ConnectionLost)
                                               .Subscribe(),
                                             Observable
                                               .FromEvent<Func<Exception, Task>, Exception>(action => ex =>
                                                                                            {
                                                                                              action(ex);

                                                                                              return Task.CompletedTask;
                                                                                            },
                                                                                            f => Hub.Closed += f,
                                                                                            f => Hub.Closed -= f)
                                               .Do(ConnectionLost)
                                               .Subscribe()
                                            );

    await Connect(reconnectTimeout, cancellationToken);
  }

  class AlwaysRetryPolicy : IRetryPolicy
  {
    readonly TimeSpan _reconnectTimeout;

    public AlwaysRetryPolicy(TimeSpan reconnectTimeout)
    {
      _reconnectTimeout = reconnectTimeout;
    }

    public TimeSpan? NextRetryDelay(RetryContext retryContext)
      => _reconnectTimeout;
  }

  public async Task Stop(CancellationToken cancellationToken)
  {
    _logger.LogInformation("SignalR: Stopping connection");

    await Hub.StopAsync(cancellationToken);
  }

  async Task Connect(TimeSpan reconnectTimeout, CancellationToken cancellationToken)
  {
    while (true)
    {
      try
      {
        _logger.LogInformation("SignalR: Starting connection to {Url}", Url);
        await Hub.StartAsync(cancellationToken);

        ConnectionEstablished();

        return;
      }
      catch when (cancellationToken.IsCancellationRequested)
      {
        _logger.LogError("SignalR: Cancelled starting connection");

        return;
      }
      catch (Exception ex)
      {
        _logger.LogError("SignalR: Error starting connection");
        ConnectionLost(ex);

        await Task.Delay(reconnectTimeout, cancellationToken);
      }
    }
  }

  void ConnectionEstablished()
  {
    _logger.LogWarning("SignalR: Connection established");

    _state.OnNext(new ConnectionUp());
  }

  void ConnectionLost(Exception exception)
  {
    _logger.LogWarning(exception, "SignalR: Connection lost");

    _state.OnNext(new ConnectionDown());
  }

  public IObservable<object> State
    => _state.DistinctUntilChanged();

  public ValueTask DisposeAsync()
  {
    _subscriptions?.Dispose();

    _state.OnCompleted();

    return Hub.DisposeAsync();
  }

  record ConnectionDown;

  record ConnectionUp;
}
