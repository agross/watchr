using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Client.Console.Token
{
  interface IToken
  {
    Regex Pattern { get; }
    Func<Match, IEnumerable<TokenData>> Yield { get; }
    Func<Match, string> Replacement { get; }

  }
}