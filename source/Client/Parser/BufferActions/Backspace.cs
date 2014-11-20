using System;
using System.Collections.Generic;
using System.Linq;

namespace Client.Parser.BufferActions
{
  class Backspace : IBufferAction
  {
    readonly string _backspaces;

    public Backspace(string backspaces)
    {
      _backspaces = backspaces;
    }

    public void Apply(List<IBufferAction> buffer)
    {
      foreach (var bs in _backspaces)
      {
        var last = buffer.OfType<Text>().LastOrDefault();
        if (last == null)
        {
          return;
        }

        last.Value = last.Value.Remove(last.Value.Length - 1);

        if (String.IsNullOrEmpty(last.Value))
        {
          buffer.Remove(last);
        }
      }
    }

    public override string ToString()
    {
      return null;
    }
  }
}
