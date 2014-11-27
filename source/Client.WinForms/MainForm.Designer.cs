namespace Client.WinForms
{
  partial class MainForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.ContextMenuStrip notificationMenu;
      this.exitApplication = new System.Windows.Forms.ToolStripMenuItem();
      this.notificationIcon = new System.Windows.Forms.NotifyIcon(this.components);
      notificationMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      notificationMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // notificationMenu
      // 
      notificationMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitApplication});
      notificationMenu.Name = "notificationMenu";
      notificationMenu.Size = new System.Drawing.Size(93, 26);
      // 
      // exitApplication
      // 
      this.exitApplication.Name = "exitApplication";
      this.exitApplication.Size = new System.Drawing.Size(92, 22);
      this.exitApplication.Text = "E&xit";
      // 
      // notificationIcon
      // 
      this.notificationIcon.ContextMenuStrip = notificationMenu;
      this.notificationIcon.Visible = true;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 262);
      this.Name = "MainForm";
      this.Text = "Watchr";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
      notificationMenu.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.NotifyIcon notificationIcon;
    private System.Windows.Forms.ToolStripMenuItem exitApplication;
  }
}

