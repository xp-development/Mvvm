using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

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

      if (frameworkElement?.DataContext is IViewDeinitialized viewDeinitialized)
        await viewDeinitialized.DeinitializedAsync();

      _contentControl.Content = (FrameworkElement) content;
      frameworkElement = (FrameworkElement) _contentControl.Content;
      if (frameworkElement?.DataContext is IViewInitialized { IsInitialized: false } viewInitialized)
        await viewInitialized.InitializedAsync(parameter);

      if (frameworkElement?.DataContext is IViewLoaded viewLoaded)
        await viewLoaded.LoadedAsync(parameter);
    }

    public Task CloseAsync(object content)
    {
      return AttachAsync(new ContentControl());
    }

    public Task CloseCurrentAsync()
    {
      return AttachAsync(new ContentControl());
    }

    public object Current => _contentControl;
  }
}