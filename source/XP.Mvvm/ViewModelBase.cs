using System.Threading.Tasks;

namespace XP.Mvvm
{
  public abstract class ViewModelBase : NotifyPropertyChangedBase, IViewInitialized, IViewDeinitialized, IViewLoaded, IViewUnloading,
    IViewUnloaded
  {
    protected Dispatcher Dispatcher;
    private string _displayName;

    protected ViewModelBase()
    {
      Dispatcher = Dispatcher.CurrentDispatcher;
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

    public Task DeinitializedAsync()
    {
      return OnDeinitializedAsync();
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

    protected virtual Task OnDeinitializedAsync()
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