using System.Reactive.Disposables;
using System.Windows.Forms;

namespace Client.WinForms
{
  class AppContext : ApplicationContext
  {
    readonly CompositeDisposable _disp;

    public AppContext()
    {
      MainForm = new MainForm();
      _disp = Bootstrapper.Setup();
    }

    protected override void Dispose(bool disposing)
    {
      if (_disp != null)
      {
        _disp.Dispose();
      }

      base.Dispose(disposing);
    }
  }
}
