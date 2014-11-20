using System.Collections.Generic;

namespace Client.BufferActions
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