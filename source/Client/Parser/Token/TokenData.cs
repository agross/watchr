namespace Client.Parser.Token
{
  class TokenData
  {
    public readonly string Data;
    public readonly string Token;

    public TokenData(string token, string data)
    {
      Token = token;
      Data = data;
    }
  }
}