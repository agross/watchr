using System;

namespace Client.WinForms.Streams.Ui
{
  class ExitApplicationRequests : IObservable<object>
  {
    readonly IObservable<object> _stream;

    public ExitApplicationRequests(IExitTheApplication exit)
    {
      _stream = exit.Requests;
    }

    public IDisposable Subscribe(IObserver<object> observer)
    {
      return _stream.Subscribe(observer);
    }
  }
}
