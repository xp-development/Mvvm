using System.Threading.Tasks;
using Avalonia.Controls;
using log4net;
using XP.Mvvm.Events;
using XP.Mvvm.Regions;

namespace XP.Mvvm.Avalonia.Regions
{
  public class SingleContentRegion : IRegion
  {
    private readonly ContentControl _contentControl;
    private readonly ILog _log = LogManager.GetLogger(typeof(SingleContentRegion));
    private IEventAggregator _eventAggregator;

    public SingleContentRegion(ContentControl contentControl, IEventAggregator eventAggregator)
    {
      _eventAggregator = eventAggregator;
      _contentControl = contentControl;
    }

    public async Task AttachAsync(object content, object parameter = null)
    {
      _log.Debug($"Attach {content.GetType()}");

      var frameworkElement = _contentControl.Content as Control;
      if (frameworkElement?.DataContext is IViewUnloading viewUnloading)
      {
        var viewUnloadingEventArgs = new ViewUnloadingEventArgs();
        await viewUnloading.UnloadingAsync(viewUnloadingEventArgs);
        await _eventAggregator.PublishAsync(new UnloadingEvent(viewUnloadingEventArgs, viewUnloading));
        _log.Debug($"ViewUnloading {frameworkElement.GetType()}");
        if (viewUnloadingEventArgs.Cancel)
        {
          _log.Debug($"ViewUnloading {frameworkElement.GetType()} cancelled.");
          return;
        }
      }

      if (frameworkElement?.DataContext is IViewUnloaded viewUnloaded)
      {
        await viewUnloaded.UnloadedAsync();
        await _eventAggregator.PublishAsync(new UnloadedEvent(viewUnloaded));
        _log.Debug($"ViewUnloaded {frameworkElement.GetType()}");
      }

      if (frameworkElement?.DataContext is IViewDeinitialized viewDeinitialized)
      {
        await viewDeinitialized.DeinitializedAsync();
        await _eventAggregator.PublishAsync(new DeinitializedEvent(viewDeinitialized));
        _log.Debug($"ViewDeinitialized {frameworkElement.GetType()}");
      }

      _contentControl.Content = (Control) content;
      frameworkElement = (Control) _contentControl.Content;
      if (frameworkElement?.DataContext is IViewInitializing { IsInitialized: false } viewInitializing)
      {
        await viewInitializing.InitializingAsync(parameter);
        await _eventAggregator.PublishAsync(new InitializingEvent(parameter, viewInitializing));
        _log.Debug($"ViewInitializing {frameworkElement.GetType()}");
      }

      if (frameworkElement?.DataContext is IViewInitialized { IsInitialized: false } viewInitialized)
      {
        await viewInitialized.InitializedAsync(parameter);
        await _eventAggregator.PublishAsync(new InitializedEvent(parameter, viewInitialized));
        _log.Debug($"ViewInitialized {frameworkElement.GetType()}");
      }

      if (frameworkElement?.DataContext is IViewLoading viewLoading)
      {
        await viewLoading.LoadingAsync(parameter);
        await _eventAggregator.PublishAsync(new LoadingEvent(parameter, viewLoading));
        _log.Debug($"ViewLoading {frameworkElement.GetType()}");
      }

      if (frameworkElement?.DataContext is IViewLoaded viewLoaded)
      {
        await viewLoaded.LoadedAsync(parameter);
        await _eventAggregator.PublishAsync(new LoadedEvent(parameter, viewLoaded));
        _log.Debug($"ViewLoaded {frameworkElement.GetType()}");
      }
    }

    public Task CloseAsync(object content)
    {
      _log.Debug($"Close {content?.GetType()}");
      return AttachAsync(new ContentControl { DataContext = null });
    }

    public Task CloseCurrentAsync()
    {
      return CloseAsync(_contentControl);
    }

    public Task ReplaceCurrentWithAsync(object content, object parameter = null)
    {
      _log.Debug($"Replace {_contentControl.GetType()} with {content.GetType()}");
      return AttachAsync(content, parameter);
    }

    public object Current => _contentControl;
  }
}