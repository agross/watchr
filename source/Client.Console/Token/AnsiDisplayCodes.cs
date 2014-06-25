using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Client.Console.Token
{
  class AnsiDisplayCodes : IToken
  {
    /// <summary>
    ///  \A\u001b\[
    ///      Beginning of string
    ///      Unicode 001b
    ///      Literal [
    ///  [Code]: A named capture group. [(?:\d{1,3};?)+|]
    ///      Select from 2 alternatives
    ///          Match expression but don't capture it. [\d{1,3};?], one or more repetitions
    ///              \d{1,3};?
    ///                  Any digit, between 1 and 3 repetitions
    ///                  ;, zero or one repetitions
    ///          NULL
    ///  m
    /// </summary>
    static readonly Regex _pattern = new Regex(
      "\\A\\u001b\\[(?<Code>(?:\\d{1,3};?)+|)m",
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
          var codes = match.Groups["Code"].Value;
          if (String.IsNullOrWhiteSpace(codes))
          {
            codes = "0";
          }

          return codes.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(code => new TokenData("display", code));
        };
      }
    }
  }
}