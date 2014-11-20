using System.Collections.Generic;

namespace Client.Messages
{
  public class BlockParsed
  {
    public BlockParsed(string sessionId, IEnumerable<Line> lines)
    {
      SessionId = sessionId;
      Lines = lines;
    }

    public string SessionId { get; set; }
    public IEnumerable<Line> Lines { get; set; }
  }
}
