using System.Web.Http;

using Owin;

namespace Web
{
  public class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      app.MapSignalR();

      GlobalConfiguration.Configure(x => x.MapHttpAttributeRoutes());
    }
  }
}
