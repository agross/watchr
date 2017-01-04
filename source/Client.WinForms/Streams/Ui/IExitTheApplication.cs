using System;

namespace Client.WinForms.Streams.Ui
{
  interface IExitTheApplication
  {
    IObservable<object> Requests { get; }
  }
}
