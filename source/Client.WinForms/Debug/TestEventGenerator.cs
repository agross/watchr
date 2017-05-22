using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Client.Web;

using Minimod.RxMessageBroker;

namespace Client.WinForms.Debug
{
  class TestEventGenerator : IDisposable
  {
    readonly CompositeDisposable _subscriptions;

    public TestEventGenerator()
    {
#if DEBUG
      _subscriptions = new CompositeDisposable(
        Observable
          .Interval(TimeSpan.FromSeconds(3))
          .StartWith(42)
          .ObserveOn(ThreadPoolScheduler.Instance)
          .Do(x =>
          {
            var connectionState = ConnectionState.Disconnected + (int) (x % 3);
            RxMessageBrokerMinimod.Default.Send(connectionState);
          })
          .Subscribe());
#endif
    }

    public void Dispose()
    {
      _subscriptions?.Dispose();
    }
  }
}
