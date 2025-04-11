using System;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace XP.Mvvm.Avalonia
{
  public class Dispatcher
  {
    
    public static Dispatcher CurrentDispatcher { get; private set; }

    
    public async Task BeginInvoke(Action action)
    {
      await global::Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(action).GetTask();
    }
  }
}
