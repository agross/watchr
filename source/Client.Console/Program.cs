using System;
using System.Threading;

using NLog;

namespace Client.Console
{
  class Program
  {
    static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    static void Main(string[] args)
    {
      Logger.Info("Starting...");
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
      Logger.Info("Press Ctrl+C to exit");

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
