using System.Collections.Generic;

namespace Client.Console.BufferActions
{
  interface IBufferAction
  {
    void Apply(List<IBufferAction> buffer);
  }
}