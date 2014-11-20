using System;
using System.Collections.Concurrent;
using System.IO;

using Client.Messages;
using Client.Minimods;

namespace Client.ScreenLogs
{
  class Subscriber
  {
    class Context
    {
      public string SessionId { get; private set; }
      public long Offset { get; set; }
      public long CurrentLineIndex { get; set; }

      Context(string sessionId)
      {
        SessionId = sessionId;
        Offset = 0;
        CurrentLineIndex = 1;
      }

      public static Context For(string path)
      {
        return new Context(Path.GetFileNameWithoutExtension(path));
      }
    }

    readonly ConcurrentDictionary<string, Context> _paths = new ConcurrentDictionary<string, Context>();

    public void FileChanged(string path)
    {
      var context = _paths.GetOrAdd(path, Context.For);

      if (!File.Exists(path))
      {
        RemoveContext(path);
        RxMessageBrokerMinimod.Default.Send(new SessionTerminated(context.SessionId));
        return;
      }

      using (var reader = new UnbufferedStreamReader(new FileStream(path,
                                                                    FileMode.Open,
                                                                    FileAccess.Read,
                                                                    FileShare.ReadWrite | FileShare.Delete)))
      {
        if (reader.BaseStream.Length == context.Offset)
        {
          return;
        }

        reader.BaseStream.Seek(context.Offset, SeekOrigin.Begin);

        var block = new BlockReceived(context.SessionId, context.CurrentLineIndex);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
          block.Append(line);
          
          if (!reader.EndOfStream)
          {
            context.CurrentLineIndex += 1;
            context.Offset = reader.BaseStream.Position;
          }
        }

        RxMessageBrokerMinimod.Default.Send(block);
      }
    }

    void RemoveContext(string path)
    {
      Context context;
      _paths.TryRemove(path, out context);

      Console.WriteLine("Session {0} ended", context.SessionId);
    }
  }
}