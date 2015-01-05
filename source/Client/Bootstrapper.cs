using System;
using System.Configuration;
using System.Reactive.Disposables;

using Client.ScreenLogs;
using Client.Streams;
using Client.Web;

namespace Client
{
  public static class Bootstrapper
  {
    public static CompositeDisposable Setup()
    {
      var subscriber = new Subscriber();

      return new CompositeDisposable
      {
        new WebClient(ConfigurationManager.AppSettings["hub-url"]),
        new ParsedBlocks().Subscribe(),
        new FileChangeListener(ConfigurationManager.AppSettings["screen-logs"]).Subscribe(x => subscriber.FileChanged(x))
      };
    }
  }
}
