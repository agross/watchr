using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Client.Token
{
  class PlainText : IToken
  {
    /// <summary>
    ///  Beginning of string
    ///  [1]: A numbered capture group. [[^\u001b\u0008\r\n]+]
    ///      Any character that is NOT in this class: [\u001b\u0008\r\n], one or more repetitions
    /// </summary>
    static readonly Regex _pattern = new Regex(
      "\\A([^\\u001b\\u0008\\r\\n]+)",
      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.Multiline);

    public Regex Pattern
    {
      get
      {
        return _pattern;
      }
    }

    public Func<Match, IEnumerable<TokenData>> Yield
    {
      get
      {
        return match => new[] { new TokenData("text", match.Value) };
      }
    }
  }
}
