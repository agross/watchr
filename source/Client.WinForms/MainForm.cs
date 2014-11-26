using System.Drawing;
using System.Windows.Forms;

using Client.WinForms.Properties;

namespace Client.WinForms
{
  partial class MainForm : Form, IMainForm
  {
    public MainForm()
    {
      InitializeComponent();
    }

    public void SetState(ConnectionState connectionState)
    {
      notificationIcon.Text = connectionState.ToString();
      notificationIcon.Icon = (Icon) Resources.ResourceManager.GetObject(connectionState.ToString());
    }
  }

  interface IMainForm
  {
    void SetState(ConnectionState connectionState);
  }

  enum ConnectionState
  {
    Disconnected,
    Connecting,
    Connected
  }
}
