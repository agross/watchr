using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Client.Console.Token
{
  class RemoveCrOnlyLineEndings : IToken
  {
    /// <summary>
    ///  Carriage return
    ///  Match if suffix is absent. [\n]
    ///      New line
    /// </summary>
    static readonly Regex _pattern = new Regex(
      "\\r(?!\\n)",
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

    public Func<Match, string> Replacement
    {
      get
      {
        return match => "";
      }
    }
  }
}