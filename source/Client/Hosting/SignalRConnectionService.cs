using System.Reactive.Linq;

using Client.Web;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Client.Hosting;

public class SignalRConnectionService : IHostedService
{
  readonly ILogger<SignalRConnectionService> _logger;
  readonly Connection _connection;

  public SignalRConnectionService(ILogger<SignalRConnectionService> logger,
                 Connection connection)
  {
    _logger = logger;
    _connection = connection;
  }

  public Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Starting");

    return _connection.Start(TimeSpan.FromSeconds(5), cancellationToken);
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Stopping");

    return _connection.Stop(cancellationToken);
  }
}
