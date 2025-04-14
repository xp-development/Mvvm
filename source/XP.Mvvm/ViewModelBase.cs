﻿using System.Threading.Tasks;
using XP.Mvvm.Events;

namespace XP.Mvvm
{
  public abstract class ViewModelBase<TViewModel>
  : NotifyPropertyChangedBase, IViewInitialized, IViewLoading, IViewLoaded, IViewUnloading, IViewDeinitialized, IViewUnloaded, IViewModelDisplay
  where TViewModel : ViewModelBase<TViewModel>
  {
    private readonly IEventAggregator _eventAggregator;
    private string _displayName;

    protected ViewModelBase(IEventAggregator eventAggregator)
    {
      _eventAggregator = eventAggregator;
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
    public bool IsLoaded { get; private set; }

    public async Task InitializedAsync(object parameter = null)
    {
      IsInitialized = true;
      await _eventAggregator.PublishAsync(new ViewInitializedEvent<TViewModel>
                                          {
                                                      ViewModel = (TViewModel)this,
                                                      Parameter = parameter
                                                    });
      await OnInitializedAsync(parameter);
    }

    public Task DeinitializedAsync()
    {
      IsInitialized = false;
      return OnDeinitializedAsync();
    }

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

    protected virtual Task OnInitializedAsync(object parameter)
    {
      return Task.CompletedTask;
    }

    protected virtual Task OnDeinitializedAsync()
    {
      return Task.CompletedTask;
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