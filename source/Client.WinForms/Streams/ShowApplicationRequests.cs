using System;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace Client.WinForms.Streams
{
  class ShowApplicationRequests : IObservable<FormWindowState>
  {
    readonly IObservable<FormWindowState> _stream;

    public ShowApplicationRequests(IShowTheApplication show, IObservable<FormWindowState> windowStates)
    {
      var visibleWindowState = windowStates
        .StartWith(FormWindowState.Normal)
        .Where(x => x != FormWindowState.Minimized)
        .DistinctUntilChanged();

      _stream = show
        .Requests
        .CombineLatest(visibleWindowState, (_, state) => state);
    }

    public IDisposable Subscribe(IObserver<FormWindowState> observer)
    {
      return _stream.Subscribe(observer);
    }
  }
}
