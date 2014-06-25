using System;
using System.IO;

using Client.Console.Messages;

using Minimod.RxMessageBroker;

namespace Client.Console
{
  class Program
  {
    static void Main(string[] args)
    {
      RxMessageBrokerMinimod.Default.Register<BlockReceived>(LineReceived);
      
      var path = @"c:\Cygwin\tmp\screen*.log";
      var subscriber = new Subscriber();

      Listener
        .Register(Path.GetDirectoryName(path), Path.GetFileName(path))
        .Subscribe(subscriber.FileChanged,
                   () => { });

      System.Console.ReadKey();
    }

    static void LineReceived(BlockReceived message)
    {
      System.Console.WriteLine("LINES:");
      System.Console.WriteLine(message.Text);
    }
  }
}
