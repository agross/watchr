using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Microsoft.AspNet.SignalR.Client;

using Minimod.RxMessageBroker;

using NLog;

namespace Client.Web
{
  class Connection : IDisposable
  {
    static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    readonly ISubject<Microsoft.AspNet.SignalR.Client.Connection> _connectRequired =
      new Subject<Microsoft.AspNet.SignalR.Client.Connection>();

    readonly Microsoft.AspNet.SignalR.Client.Connection _connection;
    readonly ISubject<object> _state = new Subject<object>();
    readonly CompositeDisposable _subscriptions;

    internal Connection(Microsoft.AspNet.SignalR.Client.Connection connection)
    {
      _connection = connection;

      _subscriptions = new CompositeDisposable(
        Observable
          .FromEvent<StateChange>(x => connection.StateChanged += x,
                                  x => connection.StateChanged -= x)
          .Subscribe(ConnectionStateChange),
        Observable
          .FromEvent(x => connection.Closed += x, x => connection.Closed -= x)
          .Subscribe(ConnectionClosed)
        );
    }

    public IObservable<object> State
      => _state
        .StartWith(new ConnectionDown(_connection))
        .DistinctUntilChanged();

    public IObservable<Microsoft.AspNet.SignalR.Client.Connection> ConnectRequired
      => _connectRequired.StartWith(_connection);

    public void Dispose()
    {
      _subscriptions?.Dispose();

      _state.OnCompleted();
      _connectRequired.OnCompleted();
    }

    void ConnectionStateChange(StateChange change)
    {
      Logger.Info("SignalR: {0} -> {1}",
                  change.OldState,
                  change.NewState);

      switch (change.NewState)
      {
        case Microsoft.AspNet.SignalR.Client.ConnectionState.Connecting:
        case Microsoft.AspNet.SignalR.Client.ConnectionState.Reconnecting:
          _state.OnNext(new ConnectionDown(_connection));
          RxMessageBrokerMinimod.Default.Send(ConnectionState.Connecting);
          return;

        case Microsoft.AspNet.SignalR.Client.ConnectionState.Disconnected:
          _state.OnNext(new ConnectionDown(_connection));
          RxMessageBrokerMinimod.Default.Send(ConnectionState.Disconnected);
          return;

        case Microsoft.AspNet.SignalR.Client.ConnectionState.Connected:
          _state.OnNext(new ConnectionUp(_connection));
          RxMessageBrokerMinimod.Default.Send(ConnectionState.Connected);
          return;
      }
    }

    void ConnectionClosed(Unit unit)
    {
      Logger.Warn("SignalR: Connection closed");

      _connectRequired.OnNext(_connection);
    }

    internal class ConnectionDown : IEquatable<ConnectionDown>
    {
      public ConnectionDown(Microsoft.AspNet.SignalR.Client.Connection connection)
      {
        Connection = connection;
      }

      Microsoft.AspNet.SignalR.Client.Connection Connection { get; }

      public bool Equals(ConnectionDown other)
      {
        if (ReferenceEquals(null, other))
        {
          return false;
        }
        if (ReferenceEquals(this, other))
        {
          return true;
        }
        return Equals(Connection.Url, other.Connection.Url) &&
               Equals(Connection.QueryString, other.Connection.QueryString);
      }

      public override bool Equals(object obj)
      {
        if (ReferenceEquals(null, obj))
        {
          return false;
        }
        if (ReferenceEquals(this, obj))
        {
          return true;
        }
        if (obj.GetType() != GetType())
        {
          return false;
        }
        return Equals((ConnectionDown) obj);
      }

      public override int GetHashCode()
      {
        return ((Connection?.Url.GetHashCode() ?? 0) * 397)
               ^ (Connection?.QueryString?.GetHashCode() ?? 0);
      }
    }

    internal class ConnectionUp : IEquatable<ConnectionUp>
    {
      public ConnectionUp(Microsoft.AspNet.SignalR.Client.Connection connection)
      {
        Connection = connection;
      }

      Microsoft.AspNet.SignalR.Client.Connection Connection { get; }

      public bool Equals(ConnectionUp other)
      {
        if (ReferenceEquals(null, other))
        {
          return false;
        }
        if (ReferenceEquals(this, other))
        {
          return true;
        }
        return Equals(Connection.Url, other.Connection.Url) &&
               Equals(Connection.QueryString, other.Connection.QueryString);
      }

      public override bool Equals(object obj)
      {
        if (ReferenceEquals(null, obj))
        {
          return false;
        }
        if (ReferenceEquals(this, obj))
        {
          return true;
        }
        if (obj.GetType() != GetType())
        {
          return false;
        }
        return Equals((ConnectionUp) obj);
      }

      public override int GetHashCode()
      {
        return ((Connection?.Url.GetHashCode() ?? 0) * 397)
               ^ (Connection?.QueryString?.GetHashCode() ?? 0);
      }
    }
  }
}
