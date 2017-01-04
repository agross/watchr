using System;

using NLog.Config;
using NLog.LayoutRenderers;
using NLog.LayoutRenderers.Wrappers;

namespace Client.NLog
{
  [LayoutRenderer("pad")]
  [AmbientProperty("rpad")]
  [ThreadAgnostic]
  public sealed class PaddingLayoutRendererWrapper : WrapperLayoutRendererBase
  {
    int _rPad;
    const char PadCharacter = ' ';

    public int RPad
    {
      get
      {
        return Math.Abs(_rPad);
      }
      set
      {
        _rPad = value;
      }
    }

    protected override string Transform(string text)
    {
      var s = text ?? string.Empty;

      if (RPad == 0)
      {
        return s;
      }

      if (s.Length < RPad)
      {
        s = s.PadLeft(RPad, PadCharacter);
      }

      if (s.Length > RPad)
      {
        s = s.Substring(s.Length - RPad, RPad);
      }

      return s;
    }
  }
}
