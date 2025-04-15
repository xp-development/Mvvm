using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using log4net;
using XP.Mvvm.Events;

namespace XP.Mvvm.Avalonia.Regions
{
  public class SingleContentRegion : RegionBase
  {
    private readonly ContentControl _contentControl;
    private readonly ILog _log = LogManager.GetLogger(typeof(SingleContentRegion));
    private IEventAggregator _eventAggregator;

    public SingleContentRegion(IEventAggregator eventAggregator, ContentControl contentControl)
    : base(eventAggregator)
    {
      _eventAggregator = eventAggregator;
      _contentControl = contentControl;
    }

    public override async Task AttachAsync(object content, object parameter = null)
    {
      _log.Debug($"Attach {content.GetType()}");

      var frameworkElement = _contentControl.Content as Control;
      if (frameworkElement?.DataContext is IViewUnloading viewUnloading)
      {
        var viewUnloadingEventArgs = new ViewUnloadingEventArgs();
        await viewUnloading.UnloadingAsync(viewUnloadingEventArgs);
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
        _log.Debug($"ViewUnloaded {frameworkElement.GetType()}");
      }

      await PublishEventMessage(typeof(ViewDeinitializedEvent<>), frameworkElement.DataContext, null);
      
      _contentControl.Content = (Control) content;
      frameworkElement = (Control) _contentControl.Content;
      await PublishEventMessage(typeof(ViewInitializedEvent<>), frameworkElement.DataContext, parameter);
        
      if (frameworkElement?.DataContext is IViewLoading viewLoading)
      {
        await viewLoading.LoadingAsync(parameter);
        _log.Debug($"ViewLoading {frameworkElement.GetType()}");
      }

      if (frameworkElement?.DataContext is IViewLoaded viewLoaded)
      {
        await viewLoaded.LoadedAsync(parameter);
        _log.Debug($"ViewLoaded {frameworkElement.GetType()}");
      }
    }

    public override Task CloseAsync(object content)
    {
      _log.Debug($"Close {content?.GetType()}");
      return AttachAsync(new ContentControl { DataContext = null });
    }

    public override Task CloseCurrentAsync()
    {
      return CloseAsync(_contentControl);
    }

    public override Task ReplaceCurrentWithAsync(object content, object parameter = null)
    {
      _log.Debug($"Replace {_contentControl.GetType()} with {content.GetType()}");
      return AttachAsync(content, parameter);
    }

    public override object Current => _contentControl;
  }
}