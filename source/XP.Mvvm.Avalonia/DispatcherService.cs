using System;
using System.Threading.Tasks;

namespace XP.Mvvm.Avalonia;

public class DispatcherService : IDispatcherService
{
    public async Task BeginInvoke(Action action)
    {
        await global::Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(action);
    }
}