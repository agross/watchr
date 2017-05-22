using System;
using System.Reactive.Linq;

using Client.Messages;

using Minimod.RxMessageBroker;

namespace Client.Web
{
  class Messages
  {
    internal Messages()
    {
      var texts = RxMessageBrokerMinimod.Default.Stream.OfType<TextReceived>().Cast<object>();
      var terminates = RxMessageBrokerMinimod.Default.Stream.OfType<SessionTerminated>().Cast<object>();

      Stream = texts.Merge(terminates);
    }

    public IObservable<object> Stream { get; }
  }
}
