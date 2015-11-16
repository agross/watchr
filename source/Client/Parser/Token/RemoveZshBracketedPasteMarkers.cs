using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Client.Parser.Token
{
  class RemoveZshBracketedPasteMarkers : IToken
  {
    /// <summary>
    ///  \A\u001b\[\?2004
    ///      Beginning of string
    ///      Unicode 001b
    ///      Literal [
    ///      Literal ?
    ///      2004
    ///  Any character in this class: [hl]
    /// </summary>
    static readonly Regex _pattern = new Regex(
      "\\A\\u001b\\[\\?2004[hl]",
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
