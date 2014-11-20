using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Client.Token
{
  class RemoveUninterestingEscapes : IToken
  {
    /// <summary>
    ///  \A\u001b
    ///      Beginning of string
    ///      Unicode 001b
    ///  [1]: A numbered capture group. [\]0;.*?\u0007|\[J|\[1m\u001b\[7m%\u001b\[27m\u001b\[1m\u001b\[0m\s+|(\(|\[)\p{Lu}|[=>]|\[\?1\w]
    ///      Select from 6 alternatives
    ///          \]0;.*?\u0007
    ///              Literal ]
    ///              0;
    ///              Any character, any number of repetitions, as few as possible
    ///              Unicode 0007
    ///          \[J
    ///              Literal [
    ///              J
    ///          \[1m\u001b\[7m%\u001b\[27m\u001b\[1m\u001b\[0m\s+
    ///              Literal [
    ///              1m
    ///              Unicode 001b
    ///              Literal [
    ///              7m%
    ///              Unicode 001b
    ///              Literal [
    ///              27m
    ///              Unicode 001b
    ///              Literal [
    ///              1m
    ///              Unicode 001b
    ///              Literal [
    ///              0m
    ///              Whitespace, one or more repetitions
    ///          (\(|\[)\p{Lu}
    ///              [2]: A numbered capture group. [\(|\[]
    ///                  Select from 2 alternatives
    ///                      Literal (
    ///                      Literal [
    ///              Any character from a Unicode character class: "Uppercase Letter"
    ///          Any character in this class: [=>]
    ///          \[\?1\w
    ///              Literal [
    ///              Literal ?
    ///              1
    ///              Alphanumeric
    /// </summary>
    static readonly Regex _pattern = new Regex(
      "\\A\\u001b(\\]0;.*?\\u0007|\\[J|\\[1m\\u001b\\[7m%\\u001b\\[27m\\u001b\\[1m\\u001b\\[0m\\s+|(\\(|\\[)\\p{Lu}|[=>]|\\[\\?1\\w)",
      RegexOptions.CultureInvariant | RegexOptions.Compiled);

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