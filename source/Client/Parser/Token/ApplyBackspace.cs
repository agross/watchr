using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Client.Parser.Token
{
  class ApplyBackspace : IToken
  {
    /// <summary>
    ///  \A\u0008+
    ///      Beginning of string
    ///      Unicode 0008, one or more repetitions
    /// </summary>
    static readonly Regex _pattern = new Regex(
      "\\A(?<BS>\\u0008+)",
      RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

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
        return match => new[] { new TokenData("backspace", match.Groups["BS"].Value) };
      }
    }
  }
}
