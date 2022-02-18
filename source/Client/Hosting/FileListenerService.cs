using System.Reactive.Linq;

using Client.ShellLogs;
using Client.Web;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Client.Hosting;

public class FileListenerService : IHostedService
{
  readonly ILogger<FileListenerService> _logger;
  readonly Listener _listener;
  readonly Subscriber _subscriber;
  readonly Publisher _publisher;
  IDisposable _subscription;

  public FileListenerService(ILogger<FileListenerService> logger,
                             Listener listener,
                             Subscriber subscriber,
                             Publisher publisher)
  {
    _logger = logger;
    _listener = listener;
    _subscriber = subscriber;
    _publisher = publisher;
  }

  public Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Starting");

    _publisher.Start();

    _subscription = _listener.StartListening()
                             .Do(x => _subscriber.FileChanged(x))
                             .Subscribe();

    return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Stopping");

    _subscription.Dispose();
    _publisher.Dispose();

    return Task.CompletedTask;
  }
}
