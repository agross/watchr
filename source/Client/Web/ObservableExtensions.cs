using System.Reactive.Linq;

namespace Client.Web;

static class ObservableExtensions
{
  internal static IObservable<T> BufferUntil<T>(this IObservable<T> instance,
                                                IObservable<T> gateStream,
                                                Func<int, bool> shouldBuffer)
  {
    return instance
           .Window(gateStream)
           .Select((d, index) =>
           {
             if (shouldBuffer(index))
             {
               return d.ToList().SelectMany(x => x);
             }

             return d;
           })
           .Concat();
  }
}
