using System.Collections.Generic;

namespace Client.Parser.BufferActions
{
  class Control : IBufferAction
  {
    readonly string _text;

    public Control(string text)
    {
      _text = text;
    }

    public void Apply(List<IBufferAction> buffer)
    {
    }

    public override string ToString()
    {
      return _text;
    }
  }
}
