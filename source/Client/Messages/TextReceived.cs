namespace Client.Messages
{
  class TextReceived
  {
    public TextReceived(string sessionId, long startOffset, long endOffset, string text)
    {
      SessionId = sessionId;
      StartOffset = startOffset;
      EndOffset = endOffset;
      Text = text;
    }

    public string SessionId { get; }
    public long StartOffset { get; }
    public long EndOffset { get; }
    public string Text { get; }
  }
}
