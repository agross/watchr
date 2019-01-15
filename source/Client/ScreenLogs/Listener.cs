using System;
using System.IO;
using System.Linq;
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

        var watcher = CreateFileSystemWatcher(_path, _filter);
        disp.Add(watcher);

        var sources =
          new[]
          {
            ForceRefresh(_path, _filter, TimeSpan.FromSeconds(3)),
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

    IObservable<EventPattern<FileSystemEventArgs>> ForceRefresh(string path, string filter, TimeSpan tick)
    {
      return Observable.Timer(TimeSpan.Zero, tick)
                       .SelectMany(_ => Directory.GetFiles(path, filter)
                                                 .ToArray()
                                                 .Select(f => new FileSystemEventArgs(WatcherChangeTypes.Changed,
                                                                                      Path.GetDirectoryName(f),
                                                                                      Path.GetFileName(f))))
                       .Select(e => new EventPattern<FileSystemEventArgs>(this, e));
    }

    static FileSystemWatcher CreateFileSystemWatcher(string path, string filter)
    {
      return new FileSystemWatcher(path, filter)
      {
        NotifyFilter = NotifyFilters.CreationTime |
                       NotifyFilters.LastWrite |
                       NotifyFilters.FileName |
                       NotifyFilters.Size
      };
    }
  }
}
