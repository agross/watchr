using System.Collections.Generic;

namespace Client.BufferActions
{
  interface IBufferAction
  {
    void Apply(List<IBufferAction> buffer);
  }
}