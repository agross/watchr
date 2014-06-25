using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Client.Demo.Messages;

using Minimod.RxMessageBroker;

namespace Client.Demo.Fake
{
  static class Fake
  {
    public static void Setup(CompositeDisposable disp)
    {
      var shells = CreateShells().ToArray();
      foreach (var shell in shells)
      {
        disp.Add(shell);
      }

      disp.Add(StartTimerFor(shells));
    }

    static IEnumerable<Shell> CreateShells()
    {
      var number = Probability.Next(1, 5);

      for (var i = 0; i < number; i++)
      {
        yield return new Shell();
      }
    }

    static IDisposable StartTimerFor(Shell[] shells)
    {
      System.Console.WriteLine("Starting timer");

      return Observable
        .Interval(TimeSpan.FromSeconds(2))
        .Select(_ => shells[Probability.Next(0, shells.Length)])
        .Do(shell => RxMessageBrokerMinimod.Default.Send(new Tick(shell.SessionId)))
        .Subscribe();
    }
  }
}