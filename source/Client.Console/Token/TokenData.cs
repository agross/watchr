namespace Client.Console.Token
{
  class TokenData
  {
    public readonly string Data;
    public string Token;

    public TokenData(string token, string data)
    {
      Token = token;
      Data = data;
    }
  }
}