using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XP.Mvvm.Regions
{
  public class SingleContentRegion : IRegion
  {
    private readonly ContentView _contentControl;

    public SingleContentRegion(ContentView contentControl)
    {
      _contentControl = contentControl;
    }

    public async Task AttachAsync(object content, object parameter = null)
    {
      var frameworkElement = _contentControl.Content as BindableObject;
      if (frameworkElement?.BindingContext is IViewUnloading viewUnloading)
      {
        var viewUnloadingEventArgs = new ViewUnloadingEventArgs();
        await viewUnloading.UnloadingAsync(viewUnloadingEventArgs);
        if (viewUnloadingEventArgs.Cancel)
          return;
      }

      if (frameworkElement?.BindingContext is IViewUnloaded viewUnloaded)
        await viewUnloaded.UnloadedAsync();

      _contentControl.Content = (View) content;
      frameworkElement = _contentControl.Content;
      if (frameworkElement?.BindingContext is IViewInitialized viewInitialized && !viewInitialized.IsInitialized)
        await viewInitialized.InitializedAsync(parameter);

      if (frameworkElement?.BindingContext is IViewLoaded viewLoaded)
        await viewLoaded.LoadedAsync(parameter);
    }
  }
}