using System;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;

namespace XP.Mvvm
{
  public class Dispatcher
  {
    private static DispatcherQueue _coreDispatcher;

    private Dispatcher(DispatcherQueue coreDispatcher)
    {
      _coreDispatcher = coreDispatcher;
    }

    public static Dispatcher CurrentDispatcher { get; private set; }

    public static void SetDispatcher(DispatcherQueue coreDispatcher)
    {
      CurrentDispatcher = new Dispatcher(coreDispatcher);
    }

    public async Task BeginInvoke(Action action)
    {
      var tcs = new TaskCompletionSource();
      _coreDispatcher.TryEnqueue(
        () =>
        {
          try
          {

            action();
            tcs.TrySetResult();
          }
          catch (Exception ex)
          {
            tcs.TrySetException(ex);
          }
        });

      await tcs.Task;
    }
  }
}
