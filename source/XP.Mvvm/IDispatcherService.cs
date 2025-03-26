using System;
using System.Threading.Tasks;

namespace XP.Mvvm;

public interface IDispatcherService
{
    Task BeginInvoke(Action action);
    Task BeginInvokeAsync(Func<Task> action);
}