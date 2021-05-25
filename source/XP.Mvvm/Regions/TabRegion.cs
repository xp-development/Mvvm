using System.Threading.Tasks;
using Xamarin.Forms;

namespace XP.Mvvm.Regions
{
  public class TabRegion : IRegion
  {
    private readonly TabbedPage _tabControl;

    public TabRegion(TabbedPage tabControl)
    {
      _tabControl = tabControl;
    }

    public async Task AttachAsync(object content, object parameter = null)
    {
      if (_tabControl.SelectedItem != null)
      {
        var frameworkElement = _tabControl.SelectedItem as BindableObject;
        if (frameworkElement?.BindingContext is IViewUnloading viewUnloading)
        {
          var viewUnloadingEventArgs = new ViewUnloadingEventArgs();
          await viewUnloading.UnloadingAsync(viewUnloadingEventArgs);
          if (viewUnloadingEventArgs.Cancel)
            return;
        }

        if (frameworkElement?.BindingContext is IViewUnloaded viewUnloaded)
          await viewUnloaded.UnloadedAsync();
      }

      if (_tabControl.Children.Contains(content as Page))
      {
        _tabControl.SelectedItem = content;
        
        var frameworkElement = (BindableObject) content;
        if (frameworkElement?.BindingContext is IViewLoaded viewLoaded)
          await viewLoaded.LoadedAsync(parameter);
      }
      else
      {
        var frameworkElement = (BindableObject) content;
        _tabControl.Children.Add((Page) frameworkElement);
        if (frameworkElement?.BindingContext is IViewInitialized viewInitialized && !viewInitialized.IsInitialized)
          await viewInitialized.InitializedAsync(parameter);

        if (frameworkElement?.BindingContext is IViewLoaded viewLoaded)
          await viewLoaded.LoadedAsync(parameter);
      }
    }
  }
}