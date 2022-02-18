using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Microsoft.Extensions.Logging;

namespace Client.ShellLogs;

public class Listener
{
  readonly ILogger<Listener> _logger;
  readonly WatchOptions _watchOptions;

  public Listener(ILogger<Listener> logger, WatchOptions watchOptions)
  {
    _logger = logger;
    _watchOptions = watchOptions;
  }

  public IObservable<string> StartListening()
  {
    _logger.LogInformation("Watching {Path}", _watchOptions.Glob);

    return Observable.Create<string>(subject =>
                     {
                       var disp = new CompositeDisposable();

                       var watcher = CreateFileSystemWatcher(_watchOptions);
                       disp.Add(watcher);

                       var sources =
                         new[]
                         {
                           // Watching iTerm2 log files does not work without it. Regular echo foo > some.log works, though.
                           ForceRefresh(_watchOptions, TimeSpan.FromSeconds(3)),
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
                                     .SelectMany(e => Observable.Throw<EventPattern<FileSystemEventArgs>>(e.EventArgs
                                                                                                           .GetException())),
                         };

                       var subscription = sources
                                          .Merge()
                                          .Select(x => x.EventArgs.FullPath)
                                          .Do(x => _logger.LogDebug("File changed: {File}", x))
                                          .Synchronize(subject)
                                          .Subscribe(subject);

                       disp.Add(subscription);

                       watcher.EnableRaisingEvents = true;

                       return disp;
                     })
                     .Publish()
                     .RefCount();
  }

  IObservable<EventPattern<FileSystemEventArgs>> ForceRefresh(WatchOptions watchOptions, TimeSpan tick)
  {
    return Observable.Timer(TimeSpan.Zero, tick)
                     .SelectMany(_ => Directory.GetFiles(watchOptions.Directory, watchOptions.Pattern)
                                               .ToArray()
                                               .Select(f => new FileSystemEventArgs(WatcherChangeTypes.Changed,
                                                                                    Path.GetDirectoryName(f),
                                                                                    Path.GetFileName(f))))
                     .Select(e => new EventPattern<FileSystemEventArgs>(this, e));
  }

  static FileSystemWatcher CreateFileSystemWatcher(WatchOptions watchOptions)
    => new(watchOptions.Directory, watchOptions.Pattern)
    {
      IncludeSubdirectories = false,
      NotifyFilter = NotifyFilters.CreationTime |
                     NotifyFilters.LastWrite |
                     NotifyFilters.FileName |
                     NotifyFilters.Size,
    };
}
