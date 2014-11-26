using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Minimod.RxMessageBroker;

namespace Client.WinForms
{
  class TestEventGenerator : IDisposable
  {
    readonly CompositeDisposable _subscriptions;

    public TestEventGenerator()
    {
      _subscriptions = new CompositeDisposable(
        Observable
          .Interval(TimeSpan.FromSeconds(1))
          .ObserveOn(ThreadPoolScheduler.Instance)
          .Do(x =>
          {
            var connectionState = ConnectionState.Disconnected + (int) (x % 3);
            RxMessageBrokerMinimod.Default.Send(connectionState);
          })
          .Subscribe())
        ;
    }

    public void Dispose()
    {
      _subscriptions.Dispose();
    }
  }
}
