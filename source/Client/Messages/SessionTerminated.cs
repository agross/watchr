namespace Client.Messages
{
  public class SessionTerminated
  {
    public string SessionId { get; set; }

    public SessionTerminated(string sessionId)
    {
      SessionId = sessionId;
    }
  }
}