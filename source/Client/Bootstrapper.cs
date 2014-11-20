using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;

using Client.Messages;
using Client.Parser;
using Client.ScreenLogs;
using Client.Web;

using Minimod.RxMessageBroker;

namespace Client
{
  public class Bootstrapper
  {
    public static CompositeDisposable Setup()
    {
      var subscriber = new Subscriber();

      return new CompositeDisposable
      {
        new WebClient(),
        HtmlConverter(),
        FileChangeListener(ConfigurationManager.AppSettings["screen-logs"], subscriber)
      };
    }

    static IDisposable HtmlConverter()
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
          Console.WriteLine("Session {0}: {1}",
                                   block.SessionId,
                                   ex.Message);
        }
      });
    }

    static IDisposable FileChangeListener(string path, Subscriber subscriber)
    {
      return Listener
        .Register(Path.GetDirectoryName(path), Path.GetFileName(path))
        .Subscribe(subscriber.FileChanged);
    }
  }
}
