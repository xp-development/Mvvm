using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace XP.Mvvm.Regions
{
  public class SingleContentRegion : IRegion
  {
    private readonly ContentControl _contentControl;

    public SingleContentRegion(ContentControl contentControl)
    {
      _contentControl = contentControl;
    }

    public async Task AttachAsync(object content, object parameter = null)
    {
      var frameworkElement = _contentControl.Content as FrameworkElement;
      if (frameworkElement?.DataContext is IViewUnloading viewUnloading)
      {
        var viewUnloadingEventArgs = new ViewUnloadingEventArgs();
        await viewUnloading.UnloadingAsync(viewUnloadingEventArgs);
        if (viewUnloadingEventArgs.Cancel)
          return;
      }

      if (frameworkElement?.DataContext is IViewUnloaded viewUnloaded)
        await viewUnloaded.UnloadedAsync();

      _contentControl.Content = (FrameworkElement) content;
      frameworkElement = (FrameworkElement) _contentControl.Content;
      if (frameworkElement?.DataContext is IViewInitialized viewInitialized && !viewInitialized.IsInitialized)
        await viewInitialized.InitializedAsync(parameter);

      if (frameworkElement?.DataContext is IViewLoaded viewLoaded)
        await viewLoaded.LoadedAsync(parameter);
    }
  }
}