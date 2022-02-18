using Client.ShellLogs;
using Client.Web;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client.Hosting;

public static class HostBuilder
{
  public static IHostBuilder ConfigureClient(this IHostBuilder hostBuilder)
  {
    return hostBuilder.ConfigureServices((context, services) =>
    {
      services.AddSingleton(_ => context.Configuration.GetSection("Watch").Get<WatchOptions>());
      services.AddSingleton<Listener>();
      services.AddSingleton<Subscriber>();

      services.AddSingleton(_ => context.Configuration.GetSection("Hub").Get<HubOptions>());
      services.AddSingleton<Connection>();
      services.AddSingleton<IHostedService, SignalRConnectionService>();

      services.AddSingleton(_ => context.Configuration.GetSection("Group").Get<GroupOptions>());
      services.AddSingleton<Publisher>();
      services.AddSingleton<IHostedService, FileListenerService>();
    });
  }
}
