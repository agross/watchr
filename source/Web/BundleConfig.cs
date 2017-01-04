using System.Web.Optimization;

namespace Web
{
  public class BundleConfig
  {
    public static void RegisterBundles(BundleCollection bundles)
    {
      bundles.Add(new StyleBundle("~/assets/css")
                    .Include("~/Css/*.css"));

      bundles.Add(new ScriptBundle("~/assets/js/lib")
                    .Include("~/Scripts/jquery-{version}.js",
                             "~/Scripts/jquery.signalR-{version}.js"));

      bundles.Add(new ScriptBundle("~/assets/js/app")
                    .Include("~/Scripts/app/*.js"));
    }
  }
}
