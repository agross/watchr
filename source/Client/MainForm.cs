using System;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;

namespace Client
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();

      var path = @"c:\Cygwin\home\agross\.zsh_history";
      var offset = new FileInfo(path).Length;

      Listener
        .Register(Path.GetDirectoryName(path), Path.GetFileName(path))
        .Subscribe(async f =>
        {
          using (var file = new StreamReader(new FileStream(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
          {
            if (file.BaseStream.Length == offset)
            {
              return;
            }

            file.BaseStream.Seek(offset, SeekOrigin.Begin);

            string line;
            while ((line = file.ReadLine()) != null)
            {
              Console.WriteLine(line);

              using (var client = new HttpClient())
              {
                client.BaseAddress = new Uri("http://localhost:34530/");
                var response = await client.PostAsJsonAsync("api/console", line);
                if (response.IsSuccessStatusCode)
                {
                }
              }
            }

            offset = file.BaseStream.Position;
          }
        },
                   () => { });
    }
  }
}
