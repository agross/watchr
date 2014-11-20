namespace Client.Messages
{
  public class Line
  {
    public string Html;
    public long Index;

    public Line(long index, string html)
    {
      Index = index;
      Html = html;
    }
  }
}
