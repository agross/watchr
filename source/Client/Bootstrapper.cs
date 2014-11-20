using System;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;

using Client.Console.Messages;

using Minimod.RxMessageBroker;

namespace Client.Console
{
  public class Bootstrapper
  {
    const string ScreenLogs = @"c:\Cygwin\tmp\screen*.log";

    public static CompositeDisposable Setup()
    {
      var subscriber = new Subscriber();

      return new CompositeDisposable
      {
        new WebClient(),
        SetUpHtmlConverter(),
        SetUpFileChangeListener(ScreenLogs, subscriber)
      };
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
