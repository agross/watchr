using System;
using System.Configuration;
using System.Drawing;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;

using Client.WinForms.Debug;
using Client.WinForms.Properties;
using Client.WinForms.Streams;

using Minimod.RxMessageBroker;

namespace Client.WinForms
{
  partial class MainForm : Form, IShowTheApplication, IExitTheApplication
  {
    readonly CompositeDisposable _subscriptions;

    public MainForm()
    {
      InitializeComponent();

      InitialWindowStateFromAppSettings();

      var uiThread = new SynchronizationContextScheduler(SynchronizationContext.Current);
      
      var windowStates = new FormWindowStates(this);

      _subscriptions = new CompositeDisposable(
        windowStates
          .Where(x => x == FormWindowState.Minimized)
          .ObserveOn(uiThread)
          .Do(x => ShowInTaskbar = false)
          .Subscribe(),
        windowStates
          .Where(x => x != FormWindowState.Minimized)
          .ObserveOn(uiThread)
          .Do(x => ShowInTaskbar = true)
          .Subscribe(),
        new ShowApplicationRequests(this, windowStates)
          .ObserveOn(uiThread)
          .Do(x => WindowState = x)
          .Subscribe(),
        new ExitApplicationRequests(this)
          .ObserveOn(uiThread)
          .Do(x => Close())
          .Subscribe(),
        RxMessageBrokerMinimod.Default.Register<ConnectionState>(SetState, uiThread),
        windowStates.Connect(),
        new TestEventGenerator()
        );
    }

    IObservable<object> IExitTheApplication.Requests
    {
      get
      {
        return Observable
          .FromEventPattern(x => exitApplication.Click += x,
                            x => exitApplication.Click -= x);
      }
    }

    IObservable<object> IShowTheApplication.Requests
    {
      get
      {
        var sources =
          new[]
          {
            Observable
              .FromEventPattern(x => notificationIcon.Click += x,
                                x => notificationIcon.Click -= x),
            Observable
              .FromEventPattern(x => notificationIcon.DoubleClick += x,
                                x => notificationIcon.DoubleClick -= x)
          };

        return sources.Merge();
      }
    }

    void InitialWindowStateFromAppSettings()
    {
      WindowState = (FormWindowState) Enum.Parse(typeof(FormWindowState),
                                                 ConfigurationManager.AppSettings["initial-state"] ?? "Normal");
    }

    void SetState(ConnectionState connectionState)
    {
      notificationIcon.Text = connectionState.ToString();
      Icon = notificationIcon.Icon = (Icon) Resources.ResourceManager.GetObject(connectionState.ToString());
    }

    void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      _subscriptions.Dispose();
    }
  }
}
