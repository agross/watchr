using System;

namespace Client.Console
{
  class TokenNotSupportedException : ParserException
  {
    public TokenNotSupportedException(string token) : base(String.Format("No handler for token of type {0}", token))
    {
    }
  }
}
