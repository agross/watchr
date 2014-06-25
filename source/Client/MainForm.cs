using System;
using System.IO;
using System.Windows.Forms;

namespace Client
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();

      var path = @"c:\Cygwin\tmp\screen*.log";
      long offset = 0;

      Listener
        .Register(Path.GetDirectoryName(path), Path.GetFileName(path))
        .Subscribe(async f =>
        {
          if (!File.Exists(f))
          {
            Console.WriteLine("Session ended");
            return;
          }

          using (var file = new StreamReader(new FileStream(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
          {
            if (file.BaseStream.Length == offset)
            {
              return;
            }

            file.BaseStream.Seek(offset, SeekOrigin.Begin);

            var lines = file.ReadToEnd();
            offset = file.BaseStream.Position;
            Console.WriteLine(lines);

            //              using (var client = new HttpClient())
            //              {
            //                client.BaseAddress = new Uri("http://localhost:34530/");
            //                var response = await client.PostAsJsonAsync("api/console", line);
            //                if (response.IsSuccessStatusCode)
            //                {
            //                }
            //              }
          }
        },
                   () => { });
    }
  }
}
