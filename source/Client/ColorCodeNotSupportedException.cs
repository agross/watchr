using System;

namespace Client
{
  class ColorCodeNotSupportedException : ParserException
  {
    public ColorCodeNotSupportedException(int colorCode) : base(String.Format("Unsupported color code: {0}", colorCode))
    {
    }
  }
}
