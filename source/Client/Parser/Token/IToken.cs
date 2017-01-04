using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Client.Parser.Token
{
  interface IToken
  {
    Regex Pattern { get; }
    Func<Match, IEnumerable<TokenData>> Yield { get; }
  }
}