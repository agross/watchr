using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Client.Parser.Token
{
  class RemoveMalformedEscapes : IToken
  {
    /// <summary>
    ///  \A\u001b\[?
    ///      Beginning of string
    ///      Unicode 001b
    ///      Literal [, zero or one repetitions
    ///  Any character in this class: [\d;], between 0 and 3 repetitions
    /// </summary>
    static readonly Regex _pattern = new Regex(
      "\\A\\u001b\\[?[\\d;]{0,3}",
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
        return match => null;
      }
    }
  }
}
