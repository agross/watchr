using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Client.Parser.Token
{
  class NormalizeLineEndings : IToken
  {
    /// <summary>
    ///  Beginning of string
    ///  [1]: A numbered capture group. [\r(?!\n)|(?<!\r)\n|\r\n]
    ///      Select from 3 alternatives
    ///          \r(?!\n)
    ///              Carriage return
    ///              Match if suffix is absent. [\n]
    ///                  New line
    ///          (?<!\r)\n
    ///              Match if prefix is absent. [\r]
    ///                  Carriage return
    ///              New line
    ///          \r\n
    ///              Carriage return
    ///              New line
    /// </summary>
    static readonly Regex _pattern = new Regex(
      "\\A(\\r(?!\\n)|(?<!\\r)\\n|\\r\\n)",
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
        return match =>
        {
          if (match.Value == "\r")
          {
            return null;
          }
          return new[] { new TokenData("text", Environment.NewLine) };
        };
      }
    }
  }
}