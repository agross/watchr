using System;
using System.Linq;

using Client.Messages;
using Client.Parser;

using Minimod.RxMessageBroker;

using NLog;

namespace Client.Streams
{
  class ParsedBlocks : IObservable<BlockParsed>
  {
    static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public IDisposable Subscribe(IObserver<BlockParsed> observer)
    {
      return RxMessageBrokerMinimod.Default.Register<BlockReceived>(block =>
      {
        try
        {
          RxMessageBrokerMinimod.Default.Send(HtmlToBlock(block));
        }
        catch (ParserException ex)
        {
          Logger.Error(String.Format("Session {0}: Could not parse ANSI", block.SessionId),
                       ex.Message);
        }
      });
    }

    static BlockParsed HtmlToBlock(BlockReceived block)
    {
      var html = new AnsiToHtml(block.Text).ToHtml();

      var lines = html
        .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
        .Select((line, index) => new Line(index + block.StartingLineIndex, line));

      return new BlockParsed(block.SessionId, lines);
    }
  }
}
