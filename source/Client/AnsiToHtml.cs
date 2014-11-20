using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using Client.Console.BufferActions;
using Client.Console.Token;

namespace Client.Console
{
  public class AnsiToHtml
  {
    static readonly IEnumerable<IToken> Token = new List<IToken>
                                                {
                                                  //new RemoveCrOnlyLineEndings(),
                                                  new NormalizeLineEndings(),
                                                  new RemoveUninterestingEscapes(),
                                                  new AnsiDisplayCodes(),
                                                  new RemoveMalformedEscapes(),
                                                  new ApplyBackspace(),
                                                  new PlainText()
                                                };

    readonly Stack<string> _stack = new Stack<string>();
    readonly IEnumerable<string> _text;

    public AnsiToHtml(string text)
    {
      _text = new[] { text };
    }

    public AnsiToHtml(IEnumerable<string> text)
    {
      _text = text;
    }

    public string ToHtml()
    {
      var buffer = new List<IBufferAction>();

      foreach (var action in Each(_text))
      {
        action.Apply(buffer);
        buffer.Add(action);
      }

      return String.Join("", buffer.Select(x => x.ToString()).ToArray());
    }

    IEnumerable<IBufferAction> Each(IEnumerable<string> ansi)
    {
      foreach (var str in ansi)
      {
        foreach (var tokenData in Tokenize(str))
        {
          switch (tokenData.Token)
          {
            case "text":
              var text = tokenData.Data;
              yield return new Text(text);
              break;

            case "display":
              var code = Convert.ToInt32(tokenData.Data);
              if (code == 0)
              {
                yield return CloseTags();
                break;
              }
              if (code == 1)
              {
                yield return PushTag("b");
                break;
              }
              if (code == 3)
              {
                yield return PushTag("i");
                break;
              }
              if (code == 4)
              {
                yield return PushTag("u");
                break;
              }
              if (code == 5)
              {
                yield return PushTag("blink", "slow");
                break;
              }
              if (code == 6)
              {
                yield return PushTag("blink", "rapid");
                break;
              }
              if (code == 7)
              {
                yield return PushTagWithClass("inverse");
                break;
              }
              if (code == 8)
              {
                yield return PushTagWithClass("conceal");
                break;
              }
              if (code == 9)
              {
                yield return PushTag("strike");
                break;
              }
              if (code == 24)
              {
                yield return CloseTag("u");
                break;
              }
              if (code == 27)
              {
                yield return CloseTag("span");
                break;
              }
              if (Enumerable.Range(30, 8).Contains(code))
              {
                yield return PushTagWithClass(code - 30);
                break;
              }
              if (code == 39)
              {
                yield return PushTagWithClass("default foreground");
                break;
              }
              if (Enumerable.Range(40, 8).Contains(code))
              {
                yield return PushTagWithClass(code - 40, " background");
                break;
              }
              if (code == 49)
              {
                yield return PushTagWithClass("default background");
                break;
              }

              if (code == 7 || code == 27)
              {
                break;
              }

              throw new DisplayCodeNotSupportedException(code);

            case "backspace":
              yield return new Backspace(tokenData.Data);
              break;

            case "special":
              yield return PushTag(tokenData.Data, null, false);
              break;

            default:
              throw new TokenNotSupportedException(tokenData.Token);
          }
        }

        if (_stack.Any())
        {
          yield return CloseTags();
        }
      }
    }

    IBufferAction CloseTag(string tag)
    {
      if (_stack.Count == 0)
      {
        return new Noop();
      }
      var last = _stack.Peek();

      if (last != tag)
      {
        return new Noop();
      }

      return new Control(String.Format("</{0}>", _stack.Pop()));
    }

    IBufferAction CloseTags()
    {
      var closingTags = String.Join("", _stack.Select(tag => String.Format("</{0}>", tag)));

      _stack.Clear();

      return new Control(closingTags);
    }

    IBufferAction PushTag(string tag, string @class = null, bool closeTag = true)
    {
      if (closeTag)
      {
        _stack.Push(tag);
      }

      if (@class != null)
      {
        @class = String.Format(" class='{0}'", @class);
      }

      return new Control(String.Join("", new[] { "<", tag, @class, ">" }));
    }

    IBufferAction PushTagWithClass(int colorCode, string @classExtra = null)
    {
      return PushTag("span", CodeToClass(colorCode) + @classExtra);
    }

    IBufferAction PushTagWithClass(string @class = null)
    {
      return PushTag("span", @class);
    }

    static string CodeToClass(int code)
    {
      switch (code)
      {
        case 0:
          return "black";
        case 1:
          return "red";
        case 2:
          return "green";
        case 3:
          return "yellow";
        case 4:
          return "blue";
        case 5:
          return "magenta";
        case 6:
          return "cyan";
        case 7:
          return "white";
        default:
          throw new ColorCodeNotSupportedException(code);
      }
    }

    static IEnumerable<TokenData> Tokenize(string text)
    {
      int length;
      while ((length = text.Length) > 0)
      {
        foreach (var token in Token)
        {
          Debug.Assert(RegexOptions.Singleline.HasFlag(token.Pattern.Options) == false,
                       "Patterns with Singleline flag are not supported as we read the file line-by-line.");
          
          Debug.Assert(token.Pattern.ToString().StartsWith("\\A"),
                       "Patterns must start with \\A for performance reasons.");

          var match = token.Pattern.Match(text);
          if (!match.Success)
          {
            continue;
          }

          foreach (var tokenData in token.Yield(match) ?? new TokenData[0])
          {
            yield return tokenData;
          }

          text = text.Remove(0, match.Length);

          break;
        }

        if (text.Length == length)
        {
          break;
        }
      }
    }
  }
}
