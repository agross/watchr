using System;

namespace Client.WinForms.Streams
{
  interface IShowTheApplication
  {
    IObservable<object> Requests { get; }
  }
}
