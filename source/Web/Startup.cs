using System.Web.Http;

using Owin;

namespace Web
{
  public class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      app.MapSignalR();

      GlobalConfiguration.Configure(x => x.Routes.MapHttpRoute(name: "API Default",
                                                               routeTemplate: "api/{controller}/{id}",
                                                               defaults: new { id = RouteParameter.Optional }
                                           ));
    }
  }
}