using System.Web.Optimization;

using Microsoft.AspNet.SignalR;

using Owin;

namespace Web
{
  public class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      var config = new HubConfiguration { EnableDetailedErrors = true };
      app.MapSignalR(config);

      BundleConfig.RegisterBundles(BundleTable.Bundles);
    }
  }
}
