using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using Client.Parser.BufferActions;
using Client.Parser.Token;

namespace Client.Parser
{
  public class AnsiToHtml
  {
    static readonly IEnumerable<IToken> Token = new List<IToken>
                                                {
                                                  //new RemoveCrOnlyLineEndings(),
                                                  new NormalizeLineEndings(),
                                                  new RemoveZshBracketedPasteMarkers(),
                                                  new RemoveTerminalTitle(),
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
              switch (code)
              {
                case 0:
                  yield return CloseTags();
                  continue;
                case 1:
                  yield return PushTag("b");
                  continue;
                case 3:
                  yield return PushTag("i");
                  continue;
                case 4:
                  yield return PushTag("u");
                  continue;
                case 5:
                  yield return PushTag("blink", "slow");
                  continue;
                case 6:
                  yield return PushTag("blink", "rapid");
                  continue;
                case 7:
                  yield return PushTagWithClass("inverse");
                  continue;
                case 8:
                  yield return PushTagWithClass("conceal");
                  continue;
                case 9:
                  yield return PushTag("strike");
                  continue;
                case 22:
                  yield return CloseTag("b");
                  continue;
                case 24:
                  yield return CloseTag("u");
                  continue;
                case 27:
                  yield return CloseTag("span");
                  continue;
                case 39:
                  yield return PushTagWithClass("default foreground");
                  continue;
                case 49:
                  yield return PushTagWithClass("default background");
                  continue;
              }
              if (Enumerable.Range(30, 8).Contains(code))
              {
                yield return PushTagWithClass(code - 30);
                break;
              }
              if (Enumerable.Range(40, 8).Contains(code))
              {
                yield return PushTagWithClass(code - 40, " background");
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
