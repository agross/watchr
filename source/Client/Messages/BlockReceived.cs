using System.Text;

namespace Client.Messages
{
  class BlockReceived
  {
    readonly StringBuilder _lines = new StringBuilder();

    public BlockReceived(string sessionId, long startingLineIndex)
    {
      SessionId = sessionId;
      StartingLineIndex = startingLineIndex;
    }

    public string SessionId { get; private set; }
    public long StartingLineIndex { get; private set; }

    public string Text
    {
      get
      {
        return _lines.ToString();
      }
    }

    public void Append(string line)
    {
      
      _lines.Append(line);
    }
  }
}
