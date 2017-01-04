using System.Collections.Generic;

namespace Client.Parser.BufferActions
{
  interface IBufferAction
  {
    void Apply(List<IBufferAction> buffer);
  }
}