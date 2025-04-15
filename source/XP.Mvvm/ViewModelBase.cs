﻿using System.Threading.Tasks;

namespace XP.Mvvm
{
  public abstract class ViewModelBase<TViewModel>
  : NotifyPropertyChangedBase, IViewLoading, IViewLoaded, IViewUnloading, IViewUnloaded, IViewModelDisplay
  where TViewModel : ViewModelBase<TViewModel>
  {
    private string _displayName;
    
    public string DisplayName
    {
      get => _displayName;
      set
      {
        _displayName = value;
        InvokePropertyChanged();
      }
    }
    
    public bool IsLoaded { get; private set; }

    public Task LoadingAsync(object parameter)
    {
      return OnLoadingAsync(parameter);
    }

    public Task LoadedAsync(object parameter)
    {
      IsLoaded = true;
      return OnLoadedAsync(parameter);
    }

    public Task UnloadedAsync()
    {
      IsLoaded = false;
      return OnUnloadedAsync();
    }

    public Task UnloadingAsync(ViewUnloadingEventArgs eventArgs)
    {
      return OnUnloadingAsync(eventArgs);
    }

    protected virtual Task OnLoadingAsync(object parameter)
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