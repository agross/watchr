using System.Web.Optimization;

using Owin;

namespace Web
{
  public class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      app.MapSignalR();
      BundleConfig.RegisterBundles(BundleTable.Bundles);
    }
  }
}
