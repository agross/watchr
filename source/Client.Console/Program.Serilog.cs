using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Exceptions;
using Serilog.Templates;
using Serilog.Templates.Themes;

namespace Client.Console;

[ExcludeFromCodeCoverage]
static class Serilog
{
  const string ConsoleTemplate = "[{@t:HH:mm:ss} {@l:u3} " +
                                 "{#if Length(SourceContext) > 25}{Substring(SourceContext, Length(SourceContext) - 25)}{#else}{SourceContext,25}{#end}" +
                                 "] {@m}\n{@x}";

  internal static void Configure(HostBuilderContext hostBuilderContext,
                                 IServiceProvider serviceProvider,
                                 LoggerConfiguration loggerConfiguration)
  {
    loggerConfiguration
      .ReadFrom.Configuration(hostBuilderContext.Configuration)
      .ReadFrom.Services(serviceProvider)
      .Enrich.FromLogContext()
      .Enrich.WithExceptionDetails()
      .WriteTo
      .Console(new ExpressionTemplate(ConsoleTemplate,
                                      theme: TemplateTheme.Code));
  }
}
