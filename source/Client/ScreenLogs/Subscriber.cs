using System.Collections.Concurrent;
using System.IO;
using System.Text;

using Client.Messages;

using Minimod.RxMessageBroker;

using NLog;

namespace Client.ScreenLogs
{
  class Subscriber
  {
    static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    class Context
    {
      public string SessionId { get; }
      public long Offset { get; set; }

      Context(string sessionId)
      {
        SessionId = sessionId;
        Offset = 0;
      }

      public static Context For(string path)
      {
        return new Context(Path.GetFileNameWithoutExtension(path));
      }
    }

    readonly ConcurrentDictionary<string, Context> _paths = new ConcurrentDictionary<string, Context>();

    public void FileChanged(string path)
    {
      var context = _paths.GetOrAdd(path,
                                    s =>
                                    {
                                      var c = Context.For(s);
                                      Logger.Debug("Session {0}: Started", c.SessionId);
                                      return c;
                                    });

      if (!File.Exists(path))
      {
        RemoveContext(path);
        RxMessageBrokerMinimod.Default.Send(new SessionTerminated(context.SessionId));
        return;
      }

      using (var reader = new StreamReader(new FileStream(path,
                                                          FileMode.Open,
                                                          FileAccess.Read,
                                                          FileShare.ReadWrite | FileShare.Delete),
                                           Encoding.UTF8))
      {
        if (reader.BaseStream.Length == context.Offset)
        {
          return;
        }

        var startOffset = context.Offset;

        reader.BaseStream.Seek(startOffset, SeekOrigin.Begin);
        var @string = reader.ReadToEnd();
        context.Offset = reader.BaseStream.Position;

        var text = new TextReceived(context.SessionId,
                                    startOffset,
                                    context.Offset,
                                    @string);

        Logger.Debug("Session {0}: Text received, offset {1} to {2}",
                     context.SessionId,
                     startOffset,
                     context.Offset);
        RxMessageBrokerMinimod.Default.Send(text);
      }
    }

    void RemoveContext(string path)
    {
      Context context;
      _paths.TryRemove(path, out context);

      Logger.Debug("Session {0}: Ended", context.SessionId);
    }
  }
}