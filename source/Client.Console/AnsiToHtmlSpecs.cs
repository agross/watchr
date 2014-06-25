using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

using NUnit.Framework;

namespace Client.Console
{
  [TestFixture]
  public class AnsiToHtmlSpecs
  {
    static readonly object[] Colors = AllColors().Select(x => new object[] { x }).ToArray();

    static IEnumerable<Color> AllColors()
    {
      var colors = typeof(Color).GetFields(BindingFlags.Static | BindingFlags.Public);

      foreach (var field in colors)
      {
        yield return (Color) field.GetValue(typeof(Color));
      }
    }

    enum For
    {
      Foreground = 30,
      Background = 40
    }

    enum Style
    {
      Normal,
      Bold
    }

    public class Color
    {
      public static readonly Color Black = new Color("black", 0);
      public static readonly Color Red = new Color("red", 1);
      public static readonly Color Green = new Color("green", 2);
      public static readonly Color Yellow = new Color("yellow", 3);
      public static readonly Color Blue = new Color("blue", 4);
      public static readonly Color Magenta = new Color("magenta", 5);
      public static readonly Color Cyan = new Color("cyan", 6);
      public static readonly Color White = new Color("white", 7);
      readonly string _className;
      readonly int _index;
      readonly Style _style;
      readonly For _type;

      Color(string className, int index, For type = For.Foreground, Style style = Style.Normal)
      {
        _className = className;
        _index = index;
        _type = type;
        _style = style;
      }

      public string ClassName
      {
        get
        {
          if (_type == For.Background)
          {
            return _className + " background";
          }

          return _className;
        }
      }

      public Color Background()
      {
        return new Color(_className, _index, For.Background, _style);
      }

      public Color Foreground()
      {
        return new Color(_className, _index, For.Foreground, _style);
      }

      public Color Bold()
      {
        return new Color(_className, _index, _type, Style.Bold);
      }

      public Color Normal()
      {
        return new Color(_className, _index, _type, Style.Normal);
      }

      public override string ToString()
      {
        var style = "";
        if (_style == Style.Bold)
        {
          style = "1;";
        }
        var colorCode = ((int) _type + _index).ToString(CultureInfo.InvariantCulture);

        return "\u001b[" + style + colorCode + "m";
      }
    }

    [Test]
    public void Should_not_modify_non_ANSI()
    {
      const string Text = "hello";

      AssertEqual(Text, Text);
    }

    [Test]
    public void Should_return_plain_text_when_plain_text_is_given()
    {
      const string Text = "hello\r\nwith\r\nnewlines";

      AssertEqual(Text, Text);
    }
    
    [Test]
    public void Should_encode_Html()
    {
      const string Text = "git reset HEAD <file>";
      const string Expected = "git reset HEAD &lt;file&gt;";

      AssertEqual(Text, Expected);
    }

    [Test]
    [TestCaseSource("Colors")]
    public void Should_support_foreground_colors(Color color)
    {
      var text = "colors: " + color + "xyz";
      var expected = "colors: <span class='" + color.ClassName + "'>xyz</span>";

      AssertEqual(text, expected);
    }

    [Test]
    [TestCaseSource("Colors")]
    public void Should_support_bold_foreground_colors(Color color)
    {
      color = color.Bold();

      var text = "colors: " + color + "xyz";
      var expected = "colors: <b><span class='" + color.ClassName + "'>xyz</span></b>";

      AssertEqual(text, expected);
    }

    [Test]
    [TestCaseSource("Colors")]
    public void Should_support_background_colors(Color color)
    {
      color = color.Background();

      var text = "colors: " + color + "xyz";
      var expected = "colors: <span class='" + color.ClassName + "'>xyz</span>";

      AssertEqual(text, expected);
    }

    [Test]
    [TestCaseSource("Colors")]
    public void Should_support_bold_background_colors(Color color)
    {
      color = color.Bold().Background();

      var text = "colors: " + color + "xyz";
      var expected = "colors: <b><span class='" + color.ClassName + "'>xyz</span></b>";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_support_multiple_colors_per_line()
    {
      var black = Color.Black;
      var white = Color.White;

      var text = "colors: " + black + "black" + white + "white";
      var expected = "colors: <span class='" + black.ClassName + "'>black<span class='" + white.ClassName +
                     "'>white</span></span>";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_support_bold()
    {
      var text = "bold: \u001b[1mstuff";
      var expected = "bold: <b>stuff</b>";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_support_italic()
    {
      var text = "underline: \u001b[3mstuff";
      var expected = "underline: <i>stuff</i>";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_support_underline()
    {
      var text = "underline: \u001b[4mstuff";
      var expected = "underline: <u>stuff</u>";

      AssertEqual(text, expected);
    }
    
    [Test]
    public void Should_support_disabled_underline()
    {
      var text = "underline: \u001b[4mstuff\u001b[24mthings";
      var expected = "underline: <u>stuff</u>things";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_support_slow_blink()
    {
      var text = "blink: \u001b[5mwhat";
      var expected = "blink: <blink class='slow'>what</blink>";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_support_rapid_blink()
    {
      var text = "blink: \u001b[6mwhat";
      var expected = "blink: <blink class='rapid'>what</blink>";

      AssertEqual(text, expected);
    } 
    
    [Test]
    public void Should_support_inverse()
    {
      var text = "inverse: \u001b[7mwhat";
      var expected = "inverse: <span class='inverse'>what</span>";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_support_conceal()
    {
      var text = "conceal: \u001b[8mthat";
      var expected = "conceal: <span class='conceal'>that</span>";

      AssertEqual(text, expected);
    }
    
    [Test]
    public void Should_support_strikethrough()
    {
      var text = "strike: \u001b[9mthat";
      var expected = "strike: <strike>that</strike>";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_support_resets()
    {
      var text = "\u001b[1mthis is bold\u001b[0m, but this is not";
      var expected = "<b>this is bold</b>, but this is not";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_support_multiple_resets()
    {
      var text = "normal, \u001b[1mbold, \u001b[3mitalic, \u001b[31mred\u001b[0m, normal";
      var expected = "normal, <b>bold, <i>italic, <span class='red'>red</span></i></b>, normal";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_support_resets_without_explicit_0()
    {
      var text = "\u001b[1mthis is bold\u001b[m, but this is not";
      var expected = "<b>this is bold</b>, but this is not";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_support_multiple_attributes()
    {
      var text = "normal, \u001b[1;4;31mbold, underline, and red\u001b[0m, normal";
      var expected = "normal, <b><u><span class='red'>bold, underline, and red</span></u></b>, normal";

      AssertEqual(text, expected);
    }
    
    [Test]
    public void Should_support_multiple_attributes_with_trailing_semicolon()
    {
      var text = "normal, \u001b[1;4;31;mbold, underline, and red\u001b[0;m, normal";
      var expected = "normal, <b><u><span class='red'>bold, underline, and red</span></u></b>, normal";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_remove_malformed_sequences()
    {
      var text = "\u001b[25oops forgot the m";
      var expected = "oops forgot the m";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_support_resetting_to_the_default_foreground_color()
    {
      var text = "\u001b[30mblack\u001b[39mdefault";
      var expected = "<span class='black'>black<span class='default foreground'>default</span></span>";

      AssertEqual(text, expected);
    }
    
    [Test]
    public void Should_support_resetting_to_the_default_background_color()
    {
      var text = "\u001b[40mblack\u001b[49mdefault";
      var expected = "<span class='black background'>black<span class='default background'>default</span></span>";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_apply_backspace_characters()
    {
      var text = "a\u0008_";
      var expected = "_";

      AssertEqual(text, expected);
    }
    
    [Test]
    public void Should_apply_backspace_characters_for_HTML_encoded_characters()
    {
      var text = "&\u0008_";
      var expected = "_";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_apply_multiple_backspace_characters()
    {
      var text = "a\u0008b\u0008";
      var expected = "";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_apply_multiple_adjacent_backspace_characters()
    {
      var text = "a\u0008\u0008_";
      var expected = "_";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_support_backspace_characters_at_the_beginning_of_the_string()
    {
      var text = "\u0008\u0008_";
      var expected = "_";

      AssertEqual(text, expected);
    }
    
    [Test]
    public void Should_support_backspace_characters_with_interleaved_escape_codes()
    {
      var text = "a\u001b[1m_\u001b[0m\u0008c";
      var expected = "a<b></b>c";

      AssertEqual(text, expected);
    }
    
    [Test]
    public void Should_support_backspace_characters_across_interleaved_escape_codes()
    {
      var text = "a\u001b[1m_\u001b[0m\u0008\u0008c";
      var expected = "<b></b>c";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_remove_markers_for_executing_commands()
    {
      var text = "before\u001b]0;git status\u0007after";
      var expected = "beforeafter";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_remove_markers_for_finished_commands()
    {
      var text = "before\u001b[1m\u001b[7m%\u001b[27m\u001b[1m\u001b[0m    after";
      var expected = "beforeafter";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_remove_CR_without_LF()
    {
      var text = "foo\rbar";
      var expected = "foobar";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_normalize_line_endings()
    {
      var text = "foo\nbar\r\n";
      var expected = String.Format("foo{0}bar{0}", Environment.NewLine);

      AssertEqual(text, expected);
    }
    
    [Test]
    public void Should_remove_character_set_escapes()
    {
      var text = "before\u001b(Bafter";
      var expected = "beforeafter";

      AssertEqual(text, expected);
    }
    
    [Test]
    public void Should_remove_control_sequence_followed_by_uppercase_character()
    {
      var text = "before\u001b[Kafter";
      var expected = "beforeafter";

      AssertEqual(text, expected);
    }
    
    [Test]
    public void Should_remove_control_sequence_with_question_mark_1_character()
    {
      
      var text = "before\u001b[?1_after";
      var expected = "beforeafter";

      AssertEqual(text, expected);
    }
    
    [Test]
    public void Should_remove_control_sequence_with_equals()
    {
      
      var text = "before\u001b=after";
      var expected = "beforeafter";

      AssertEqual(text, expected);
    }
    
    [Test]
    public void Should_remove_control_sequence_with_greater_than()
    {
      
      var text = "before\u001b>after";
      var expected = "beforeafter";

      AssertEqual(text, expected);
    }

    [Test]
    public void Should_support_dumps()
    {
      var path = "c:/Cygwin/tmp/screen-*.log";
      var file = Directory.GetFiles(Path.GetDirectoryName(path), Path.GetFileName(path), SearchOption.TopDirectoryOnly).FirstOrDefault();

      if (file == null)
      {
        Assert.Inconclusive("No files matching " + path);
      }

      using (var reader = new StreamReader(new FileStream(file,
                                                          FileMode.Open,
                                                          FileAccess.Read,
                                                          FileShare.ReadWrite | FileShare.Delete)))
      {
        var dump = reader.ReadToEnd();

        var html = new AnsiToHtml(dump).ToHtml();


        html = "<html><link href='style.css' rel='stylesheet' type='text/css'></link><body><pre>" + html;
        html += "</pre></body></html>";
        File.WriteAllText(Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".html"),
                          html);
        Assert.IsNotEmpty(html);
      }
    }

    static void AssertEqual(string text, string expected)
    {
      var html = new AnsiToHtml(text).ToHtml();

      Assert.AreEqual(expected, html);
    }
  }
}
