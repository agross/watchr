using System;

namespace Client.WinForms.Streams
{
  interface IExitTheApplication
  {
    IObservable<object> Requests { get; }
  }
}
