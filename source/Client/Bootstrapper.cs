using System;
using System.Configuration;
using System.Reactive.Disposables;

using Client.ScreenLogs;
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
        new WebClient(ConfigurationManager.AppSettings["hub-url"],
                      ConfigurationManager.AppSettings["group-id"]),
        new FileChangeListener(ConfigurationManager.AppSettings["screen-logs"]).Subscribe(x => subscriber.FileChanged(x))
      };
    }
  }
}
