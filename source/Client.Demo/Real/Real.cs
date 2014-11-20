using System;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;

using Client.Console;
using Client.Console.Messages;

using Minimod.RxMessageBroker;

namespace Client.Demo.Real
{
  public class Real
  {
    public static void Setup(CompositeDisposable disp)
    {
      var path = @"c:\Cygwin\tmp\screen*.log";

      var subscriber = new Subscriber();

      disp.Add(SetUpHtmlConverter());
      disp.Add(SetUpFileChangeListener(path, subscriber));
    }

    static IDisposable SetUpHtmlConverter()
    {
      return RxMessageBrokerMinimod.Default.Register<BlockReceived>(block =>
      {
        try
        {
          var html = new AnsiToHtml(block.Text).ToHtml();

          var lines = html
            .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
            .Select((line, index) => new Line(index + block.StartingLineIndex, line));

          RxMessageBrokerMinimod.Default.Send(new BlockParsed(block.SessionId, lines));
        }
        catch (ParserException ex)
        {
          System.Console.WriteLine("Session {0}: {1}",
                                   block.SessionId,
                                   ex.Message);
        }
      });
    }

    static IDisposable SetUpFileChangeListener(string path, Subscriber subscriber)
    {
      return Listener
        .Register(Path.GetDirectoryName(path), Path.GetFileName(path))
        .Subscribe(subscriber.FileChanged);
    }
  }
}
