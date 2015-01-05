using System.Reactive.Disposables;
using System.Windows.Forms;

namespace Client.WinForms
{
  class AppContext : ApplicationContext
  {
    readonly CompositeDisposable _disp;

    public AppContext()
    {
      _disp = Bootstrapper.Setup();
      MainForm = new MainForm();
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
