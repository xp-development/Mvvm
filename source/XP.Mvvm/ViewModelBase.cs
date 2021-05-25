using System.Threading;
using System.Threading.Tasks;

namespace XP.Mvvm
{
  public abstract class ViewModelBase : NotifyPropertyChangedBase, IViewInitialized, IViewLoaded, IViewUnloading,
    IViewUnloaded
  {
    private string _displayName;
    protected SynchronizationContext SynchronizationContext;

    protected ViewModelBase()
    {
      SynchronizationContext = SynchronizationContext.Current;
    }

    public string DisplayName
    {
      get => _displayName;
      set
      {
        _displayName = value;
        InvokePropertyChanged();
      }
    }

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

    public async Task UnloadedAsync()
    {
      await OnUnloadedAsync();
    }

    public Task UnloadingAsync(ViewUnloadingEventArgs eventArgs)
    {
      return OnUnloadingAsync(eventArgs);
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