using System.Net;

using Microsoft.AspNetCore.HttpOverrides;

using Web;
using Web.Hubs;

using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
  builder.Services.AddCors(x =>
  {
    x.AddDefaultPolicy(policy => policy
                                 .SetIsOriginAllowed(_ => true)
                                 .AllowAnyMethod()
                                 .AllowAnyHeader()
                                 .AllowCredentials());
  });

  builder.Services.AddHostedService<Debugging>();
}

builder.Services
       .AddSignalR(o => o.EnableDetailedErrors = true)
       .AddHubOptions<ShellHub>(o => o.MaximumReceiveMessageSize = 512_000);

builder.Services.AddHealthChecks();
builder.Services.AddHttpLogging(logging =>
{
  new List<string>
    {
      "X-Forwarded-For",
      "X-Forwarded-Host",
      "X-Forwarded-Port",
      "X-Forwarded-Prefix",
      "X-Forwarded-Proto",
      "X-Forwarded-Server",
      "X-Real-IP",
    }
    .ForEach(x => logging.RequestHeaders.Add(x));
});

builder
  .Host
  .UseDefaultServiceProvider((_, options) =>
  {
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
  });

var app = builder.Build();

var config = app.Services.GetRequiredService<IConfiguration>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Configuration:\n{Config}",
                      ((IConfigurationRoot) config).GetDebugView());

if (builder.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseCors();
}
else
{
  // Serve pre-compiled Vue app.
  app.UseDefaultFiles();
  app.UseStaticFiles();

  app.UseRouting();
  app.MapFallbackToFile("index.html");
}

// Must run first.
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
  ForwardedHeaders = ForwardedHeaders.All,
};

// By default only loopback proxies are allowed. Does not work on docker-compose
// networks.
forwardedHeadersOptions.KnownProxies.Clear();
new List<IPNetwork>
{
  new(new IPAddress(new byte[] { 10, 0, 0, 0 }), 8),
  new(new IPAddress(new byte[] { 172, 16, 0, 0 }), 12),
  new(new IPAddress(new byte[] { 192, 168, 0, 0 }), 16),
}.ForEach(x => forwardedHeadersOptions.KnownNetworks.Add(x));

app.UseForwardedHeaders(forwardedHeadersOptions);
app.UseHealthChecks("/health");
app.UseHttpLogging();

app.MapHub<ShellHub>("/shell");

app.Run();
