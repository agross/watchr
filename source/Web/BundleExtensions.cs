using System.Collections.Generic;
using System.Web.Optimization;

namespace Web
{
  static class BundleExtensions
  {
    sealed class AsIsBundleOrderer : IBundleOrderer
    {
      public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
      {
        return files;
      }
    }

    public static Bundle ForceOrdered(this Bundle sb)
    {
      sb.Orderer = new AsIsBundleOrderer();
      return sb;
    }
  }
}
