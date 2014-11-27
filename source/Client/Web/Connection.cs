using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

using Microsoft.AspNet.SignalR.Client;

namespace Client.Web
{
  class Connection : IDisposable
  {
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
    {
      get
      {
        return _state
          .StartWith(new ConnectionDown(_connection))
          .DistinctUntilChanged();
      }
    }

    public IObservable<Microsoft.AspNet.SignalR.Client.Connection> ConnectRequired
    {
      get
      {
        return _connectRequired
          .StartWith(_connection);
      }
    }

    public void Dispose()
    {
      if (_subscriptions != null)
      {
        _subscriptions.Dispose();
      }

      _state.OnCompleted();
      _connectRequired.OnCompleted();
    }

    void ConnectionStateChange(StateChange change)
    {
      Console.WriteLine("{0} SignalR: {1} -> {2}",
                        Thread.CurrentThread.ManagedThreadId,
                        change.OldState,
                        change.NewState);

      if (change.NewState == ConnectionState.Connecting ||
          change.NewState == ConnectionState.Disconnected ||
          change.NewState == ConnectionState.Reconnecting)
      {
        _state.OnNext(new ConnectionDown(_connection));
      }
      else
      {
        _state.OnNext(new ConnectionUp(_connection));
      }
    }

    void ConnectionClosed(Unit unit)
    {
      Console.WriteLine("{0} SignalR: Connection closed", Thread.CurrentThread.ManagedThreadId);

      _connectRequired.OnNext(_connection);
    }

    internal class ConnectionDown : IEquatable<ConnectionDown>
    {
      public ConnectionDown(Microsoft.AspNet.SignalR.Client.Connection connection)
      {
        Connection = connection;
      }

      public Microsoft.AspNet.SignalR.Client.Connection Connection { get; private set; }

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
        return ((Connection != null ? Connection.Url.GetHashCode() : 0) * 397)
               ^ ((Connection != null && Connection.QueryString != null) ? Connection.QueryString.GetHashCode() : 0);
      }
    }

    internal class ConnectionUp : IEquatable<ConnectionUp>
    {
      public ConnectionUp(Microsoft.AspNet.SignalR.Client.Connection connection)
      {
        Connection = connection;
      }

      public Microsoft.AspNet.SignalR.Client.Connection Connection { get; private set; }

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
        return ((Connection != null ? Connection.Url.GetHashCode() : 0) * 397)
               ^ ((Connection != null && Connection.QueryString != null) ? Connection.QueryString.GetHashCode() : 0);
      }
    }
  }
}
