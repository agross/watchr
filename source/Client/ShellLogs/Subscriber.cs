using System.Collections.Concurrent;
using System.Text;

using Client.Messages;

using Microsoft.Extensions.Logging;

using Minimod.RxMessageBroker;

namespace Client.ShellLogs;

public class Subscriber
{
  readonly ILogger<Subscriber> _logger;
  readonly ConcurrentDictionary<string, Context> _paths = new();

  public Subscriber(ILogger<Subscriber> logger)
  {
    _logger = logger;
  }

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
      => new(Path.GetFileNameWithoutExtension(path));
  }

  public void FileChanged(string path)
  {
    var context = Context.For(path);

    if (!File.Exists(path))
    {
      if (RemoveContext(path))
      {
        RxMessageBrokerMinimod.Default.Send(new SessionTerminated(context.SessionId));
      }

      return;
    }

    context = _paths.GetOrAdd(path,
                              _ =>
                              {
                                var newContext = Context.For(path);
                                _logger.LogDebug("Session {SessionId}: Started",
                                                 newContext.SessionId);

                                return newContext;
                              });

    using var reader = new StreamReader(new FileStream(path,
                                                       FileMode.Open,
                                                       FileAccess.Read,
                                                       FileShare.ReadWrite |
                                                       FileShare.Delete),
                                        Encoding.UTF8);
    if (reader.BaseStream.Length == context.Offset)
    {
      return;
    }

    reader.BaseStream.Seek(context.Offset, SeekOrigin.Begin);
    var buffer = new char[500000];

    int charsRead;
    while ((charsRead = reader.ReadBlock(buffer, 0, buffer.Length)) > 0)
    {
      var text = new TextReceived(context.SessionId,
                                  context.Offset,
                                  reader.BaseStream.Position,
                                  new string(buffer.Take(charsRead).ToArray()));

      _logger.LogDebug("Session {SessionId}: Text received, offset {Offset} to {Position}",
                       context.SessionId,
                       context.Offset,
                       reader.BaseStream.Position);
      RxMessageBrokerMinimod.Default.Send(text);

      context.Offset = reader.BaseStream.Position;
    }
  }

  bool RemoveContext(string path)
  {
    var removed = _paths.TryRemove(path, out var context);
    if (removed)
    {
      _logger.LogDebug("Session {SessionId}: Ended", context.SessionId);
    }

    return removed;
  }
}
