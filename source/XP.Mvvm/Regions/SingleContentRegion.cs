using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using log4net;

namespace XP.Mvvm.Regions
{
  public class SingleContentRegion : IRegion
  {
    private readonly ContentControl _contentControl;
    private readonly ILog _log = LogManager.GetLogger(typeof(SingleContentRegion));

    public SingleContentRegion(ContentControl contentControl)
    {
      _contentControl = contentControl;
    }

    public async Task AttachAsync(object content, object parameter = null)
    {
      _log.Debug($"Attach {content.GetType()}");

      var frameworkElement = _contentControl.Content as FrameworkElement;
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

      if (frameworkElement?.DataContext is IViewDeinitialized viewDeinitialized)
      {
        await viewDeinitialized.DeinitializedAsync();
        _log.Debug($"ViewDeinitialized {frameworkElement.GetType()}");
      }

      _contentControl.Content = (FrameworkElement) content;
      frameworkElement = (FrameworkElement) _contentControl.Content;
      if (frameworkElement?.DataContext is IViewInitialized { IsInitialized: false } viewInitialized)
      {
        await viewInitialized.InitializedAsync(parameter);
        _log.Debug($"ViewInitialized {frameworkElement.GetType()}");
      }

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

    public Task CloseAsync(object content)
    {
      _log.Debug($"Close {content.GetType()}");
      return AttachAsync(new ContentControl());
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