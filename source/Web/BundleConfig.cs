using System.Web.Optimization;

namespace Web
{
  class BundleConfig
  {
    public static void RegisterBundles(BundleCollection bundles)
    {
      bundles.Add(new StyleBundle("~/assets/css")
                    .Include("~/Css/style.css")
                    .Include("~/Css/xterm.css")
                    .Include("~/Css/xterm-custom.css")
                    .ForceOrdered());

      bundles.Add(new ScriptBundle("~/assets/js/lib")
                    .Include("~/Scripts/jquery/jquery.js")
                    .Include("~/Scripts/signalr/jquery.signalR.min.js")
                    .Include("~/Scripts/xterm/xterm.js")
                    .Include("~/Scripts/xterm/fit.js")
                    .Include("~/Scripts/css-element-queries/*.js")
                    .ForceOrdered());

      bundles.Add(new ScriptBundle("~/assets/js/app")
                    .Include("~/Scripts/app/modules/*.js")
                    .Include("~/Scripts/app/*.js")
                    .ForceOrdered());
    }
  }
}
