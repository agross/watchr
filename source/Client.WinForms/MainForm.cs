using System;
using System.Configuration;
using System.Drawing;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using Client.Web;
using Client.WinForms.Debug;
using Client.WinForms.Messages;
using Client.WinForms.Properties;
using Client.WinForms.Streams.Ui;

using Minimod.RxMessageBroker;

using NLog;

namespace Client.WinForms
{
  partial class MainForm : Form, IShowTheApplication, IExitTheApplication
  {
    readonly CompositeDisposable _subscriptions;

    public MainForm()
    {
      InitializeComponent();

      DoubleBuffer(Log);

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
        RxMessageBrokerMinimod.Default.Register<ConnectionState>(SetStateIcon, uiThread),
        windowStates.Connect(),
        new TestEventGenerator(),
        RxMessageBrokerMinimod.Default.Register<LogMessage>(AppendLogMessage, uiThread)
        );
    }

    static void DoubleBuffer(ListView listView)
    {
      var doubleBufferPropertyInfo = listView.GetType()
                                             .GetProperty("DoubleBuffered",
                                                          BindingFlags.Instance | BindingFlags.NonPublic);
      doubleBufferPropertyInfo.SetValue(listView, true, null);
    }

    void AppendLogMessage(LogMessage message)
    {
      var item = new ListViewItem
      {
        Text = (Log.Items.Count + 1).ToString(),
        ForeColor = LevelToColor(message) ?? Log.ForeColor
      };

      Log.BeginUpdate();

      foreach (var prop in message.GetType().GetProperties())
      {
        if (!Log.Columns.ContainsKey(prop.Name))
        {
          Log.Columns.Add(prop.Name, prop.Name);
        }

        item.SubItems.Add(prop.GetValue(message).ToString());
      }

      Log.Items.Add(item).EnsureVisible();
      
      Log.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
      
      Log.EndUpdate();
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

    void SetStateIcon(ConnectionState connectionState)
    {
      notificationIcon.Text = connectionState.ToString();
      Icon = notificationIcon.Icon = (Icon) Resources.ResourceManager.GetObject(connectionState.ToString());
    }

    void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      _subscriptions.Dispose();
    }

    static Color? LevelToColor(LogMessage item)
    {
      if (item.Level == LogLevel.Error)
      {
        return Color.Red;
      }

      if (item.Level == LogLevel.Warn)
      {
        return Color.DarkOrange;
      }

      if (item.Level == LogLevel.Debug)
      {
        return Color.DarkGray;
      }

      return null;
    }
  }
}
