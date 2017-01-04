using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Client.ScreenLogs
{
  class Listener
  {
    readonly string _path;
    readonly string _filter;

    public Listener(string path, string filter = null)
    {
      _path = path;
      _filter = filter;
    }

    public IObservable<string> Register()
    {
      return Observable.Create<string>(subject =>
      {
        var disp = new CompositeDisposable();

        var timer = ForceRefreshOfFileSystemEntries(_path, _filter);
        disp.Add(timer);

        var watcher = CreateFileSystemWatcher(_path, _filter);
        disp.Add(watcher);

        var sources =
          new[]
          {
            Observable.FromEventPattern
              <FileSystemEventHandler, FileSystemEventArgs>(x => watcher.Changed += x,
                                                            x => watcher.Changed -= x),
            Observable.FromEventPattern
              <FileSystemEventHandler, FileSystemEventArgs>(x => watcher.Created += x,
                                                            x => watcher.Created -= x),
            Observable.FromEventPattern
              <FileSystemEventHandler, FileSystemEventArgs>(x => watcher.Deleted += x,
                                                            x => watcher.Deleted -= x),
            Observable.FromEventPattern<ErrorEventArgs>(watcher, "Error")
                      .SelectMany(e => Observable.Throw<EventPattern<FileSystemEventArgs>>(e.EventArgs.GetException()))
          };

        var subscription = sources
          .Merge()
          .Select(x => x.EventArgs.FullPath)
          .Synchronize(subject)
          .Subscribe(subject);

        disp.Add(subscription);

        watcher.EnableRaisingEvents = true;
        return disp;
      }).Publish().RefCount();
    }

    static IDisposable ForceRefreshOfFileSystemEntries(string path, string filter)
    {
      return Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
                       .Subscribe(_ => Array.ForEach(Directory.GetFiles(path, filter ?? "*"),
                                                     x => new FileInfo(x).Refresh()));
    }

    static FileSystemWatcher CreateFileSystemWatcher(string path, string filter)
    {
      return new FileSystemWatcher(path, filter)
      {
        NotifyFilter = NotifyFilters.CreationTime |
                       NotifyFilters.LastWrite |
                       NotifyFilters.DirectoryName |
                       NotifyFilters.FileName |
                       NotifyFilters.Size |
                       NotifyFilters.Attributes
      };
    }
  }
}
