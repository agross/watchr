using System;

namespace Client
{
  [Serializable]
  class DisplayCodeNotSupportedException : ParserException
  {
    public DisplayCodeNotSupportedException(int code) : base(String.Format("Unsupported display code: {0}", code))
    {
    }
  }
}
