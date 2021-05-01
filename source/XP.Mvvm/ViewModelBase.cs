using System.Threading.Tasks;

namespace XP.Mvvm
{
  public abstract class ViewModelBase : NotifyPropertyChangedBase, IViewInitialized, IViewLoaded, IViewUnloading, IViewUnloaded
  {
    public bool IsInitialized { get; private set; }

    public Task InitializedAsync(object parameter = null)
    {
      IsInitialized = true;
      return OnInitializedAsync(parameter);
    }

    public Task LoadedAsync(object parameter)
    {
      return OnLoadedAsync(parameter);
    }

    public Task UnloadingAsync(ViewUnloadingEventArgs eventArgs)
    {
      return OnUnloadingAsync(eventArgs);
    }

    public async Task UnloadedAsync()
    {
      await OnUnloadedAsync();
    }

    protected virtual Task OnInitializedAsync(object parameter)
    {
      return Task.CompletedTask;
    }

    protected virtual Task OnLoadedAsync(object parameter)
    {
      return Task.CompletedTask;
    }

    protected virtual Task OnUnloadingAsync(ViewUnloadingEventArgs eventArgs)
    {
      return Task.CompletedTask;
    }

    protected virtual Task OnUnloadedAsync()
    {
      return Task.CompletedTask;
    }
  }
}