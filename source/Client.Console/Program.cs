using Client.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Debugging;

SelfLog.Enable(Console.Error);
Log.Logger = new LoggerConfiguration()
             .WriteTo.Console()
             .CreateBootstrapLogger();

var host = Host
           .CreateDefaultBuilder(args)
           .UseConsoleLifetime()
           .UseDefaultServiceProvider((_, options) =>
           {
             options.ValidateScopes = true;
             options.ValidateOnBuild = true;
           })
           .UseSerilog(Client.Console.Serilog.Configure)
           .ConfigureClient()
           .Build();

var config = host.Services.GetRequiredService<IConfiguration>();
Log.ForContext<Program>().Information("Configuration:\n{Config}",
                                      ((IConfigurationRoot) config).GetDebugView());

host.Run();
