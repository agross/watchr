using System;
using System.Reactive.Linq;

using Client.Messages;

using Minimod.RxMessageBroker;

namespace Client.Web
{
  class Messages
  {
    readonly IObservable<object> _stream;

    internal Messages()
    {
      var blocks = RxMessageBrokerMinimod.Default.Stream.OfType<BlockParsed>().Cast<object>();
      var terminates = RxMessageBrokerMinimod.Default.Stream.OfType<SessionTerminated>().Cast<object>();

      _stream = blocks.Merge(terminates);
    }

    public IObservable<object> Stream
    {
      get
      {
        return _stream;
      }
    }
  }
}
