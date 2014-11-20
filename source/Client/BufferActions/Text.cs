using System.Collections.Generic;
using System.Net;

namespace Client.BufferActions
{
  class Text : IBufferAction
  {
    public Text(string text)
    {
      Value = text;
    }

    public string Value { get; set; }

    public void Apply(List<IBufferAction> buffer)
    {
    }

    public override string ToString()
    {
      return WebUtility.HtmlEncode(Value);
    }
  }
}
