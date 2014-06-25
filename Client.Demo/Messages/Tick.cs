namespace Client.Demo.Messages
{
  class Tick
  {
    public Tick(string sessionId)
    {
      SessionId = sessionId;
    }

    public string SessionId { get; private set; }
  }
}
