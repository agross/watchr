using System;

namespace Client.WinForms.Streams.Ui
{
  interface IShowTheApplication
  {
    IObservable<object> Requests { get; }
  }
}
