namespace Client.Messages
{
  public class SessionTerminated
  {
    public SessionTerminated(string sessionId)
    {
      SessionId = sessionId;
    }

    public string SessionId { get; set; }
  }
}
