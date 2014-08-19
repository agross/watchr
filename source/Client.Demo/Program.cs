using System.Reactive.Disposables;

namespace Client.Demo
{
  class Program
  {
    static void Main(string[] args)
    {
      var disp = new CompositeDisposable();

      //Fake.Fake.Setup(disp);
      Real.Real.Setup(disp);

      disp.Add(new WebClient());

      System.Console.WriteLine("Press any key to exit");
      System.Console.ReadKey();

      disp.Dispose();
    }
  }
}
