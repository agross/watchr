using System.Reactive.Disposables;

using Client.Console;

namespace Client.Demo
{
  class Program
  {
    static void Main(string[] args)
    {
      System.Console.WriteLine("Starting...");
      CompositeDisposable disp = null;
      try
      {
        disp = Bootstrapper.Setup();

        System.Console.WriteLine("Press any key to exit");
        System.Console.ReadKey();
      }
      finally
      {
        if (disp != null)
        {
          disp.Dispose();
        }
      }
    }
  }
}
