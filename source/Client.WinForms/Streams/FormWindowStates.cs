using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Forms;

namespace Client.WinForms.Streams
{
  class FormWindowStates : IConnectableObservable<FormWindowState>
  {
    readonly IConnectableObservable<FormWindowState> _stream;

    public FormWindowStates(Form form)
    {
      _stream = Observable
        .FromEventPattern(x => form.Resize += x, x => form.Resize -= x)
        .Select(x => form.WindowState)
        .StartWith(form.WindowState)
        .DistinctUntilChanged()
        .Publish();
    }

    public IDisposable Connect()
    {
      return _stream.Connect();
    }

    public IDisposable Subscribe(IObserver<FormWindowState> observer)
    {
      return _stream.Subscribe(observer);
    }
  }
}
