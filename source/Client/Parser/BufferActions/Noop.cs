using System.Collections.Generic;

namespace Client.Parser.BufferActions
{
  class Noop : IBufferAction
  {
    public void Apply(List<IBufferAction> buffer)
    {
        
    }

    public override string ToString()
    {
      return null;
    }
  }
}