using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Client.Console.Token
{
  class NormalizeLineEndings : IToken
  {
    /// <summary>
    ///  [1]: A numbered capture group. [\r(?!\n)|(?<!\r)\n]
    ///      Select from 2 alternatives
    ///          \r(?!\n)
    ///              Carriage return
    ///              Match if suffix is absent. [\n]
    ///                  New line
    ///          (?<!\r)\n
    ///              Match if prefix is absent. [\r]
    ///                  Carriage return
    ///              New line
    /// </summary>
    static readonly Regex _pattern = new Regex(
      "(\\r(?!\\n)|(?<!\\r)\\n)",
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
        return match => Environment.NewLine;
      }
    }
  }
}