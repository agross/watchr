using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Client.Parser.Token
{
  class RemoveTerminalTitle : IToken
  {
    /// <summary>
    ///  \A\u001b.*?\u001b\\
    ///      Beginning of string
    ///      Unicode 001b
    ///      Any character, any number of repetitions, as few as possible
    ///      Unicode 001b
    ///      Literal \
    /// </summary>
    static readonly Regex _pattern = new Regex(
      "\\A\\u001b.*?\\u001b\\\\",
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