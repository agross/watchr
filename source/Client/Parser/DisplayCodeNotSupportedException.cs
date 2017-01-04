using System;

namespace Client.Parser
{
  [Serializable]
  class DisplayCodeNotSupportedException : ParserException
  {
    public DisplayCodeNotSupportedException(int code) : base(String.Format("Unsupported display code: {0}", code))
    {
    }
  }
}
