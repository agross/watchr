using System.Drawing;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading;
using System.Windows.Forms;

using Client.WinForms.Properties;

using Minimod.RxMessageBroker;

namespace Client.WinForms
{
  partial class MainForm : Form
  {
    CompositeDisposable _subscriptions;

    public MainForm()
    {
      InitializeComponent();

      var mainThread = new SynchronizationContextScheduler(SynchronizationContext.Current);

      _subscriptions = new CompositeDisposable(
        RxMessageBrokerMinimod.Default.Register<ConnectionState>(SetState, mainThread)
        );
    }

    void SetState(ConnectionState connectionState)
    {
      notificationIcon.Text = connectionState.ToString();
      Icon = notificationIcon.Icon = (Icon) Resources.ResourceManager.GetObject(connectionState.ToString());
    }
  }
}
