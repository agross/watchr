using System;
using System.Threading;

namespace Client.Console
{
  class Program
  {
    static void Main(string[] args)
    {
      System.Console.WriteLine("Starting...");
      IDisposable disp = null;
      try
      {
        disp = Bootstrapper.Setup();

        WaitForCtrlC();
      }
      finally
      {
        if (disp != null)
        {
          disp.Dispose();
        }
      }
    }

    static void WaitForCtrlC()
    {
      System.Console.WriteLine("Press Ctrl+C to exit");

      var quit = new ManualResetEvent(false);

      System.Console.CancelKeyPress += (o, args) =>
      {
        args.Cancel = true;
        quit.Set();
      };

      quit.WaitOne();
    }
  }
}
