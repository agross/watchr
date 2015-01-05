using System;
using System.Linq;

using Client.Messages;
using Client.Parser;

using Minimod.RxMessageBroker;

namespace Client.Streams
{
  public class ParsedBlocks : IObservable<BlockParsed>
  {
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
          Console.WriteLine("Session {0}: {1}",
                            block.SessionId,
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
