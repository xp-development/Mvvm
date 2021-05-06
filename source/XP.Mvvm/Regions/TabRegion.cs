using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace XP.Mvvm.Regions
{
  public class TabRegion : IRegion
  {
    private readonly TabControl _tabControl;

    public TabRegion(TabControl tabControl)
    {
      _tabControl = tabControl;
    }

    public async Task AttachAsync(object content, object parameter = null)
    {
      if (_tabControl.SelectedContent != null)
      {
        var frameworkElement = _tabControl.SelectedContent as FrameworkElement;
        if (frameworkElement?.DataContext is IViewUnloading viewUnloading)
        {
          var viewUnloadingEventArgs = new ViewUnloadingEventArgs();
          await viewUnloading.UnloadingAsync(viewUnloadingEventArgs);
          if (viewUnloadingEventArgs.Cancel)
            return;
        }

        if (frameworkElement?.DataContext is IViewUnloaded viewUnloaded)
          await viewUnloaded.UnloadedAsync();
      }

      if (_tabControl.Items.Contains(content))
      {
        _tabControl.Items.MoveCurrentTo(content);
        
        var frameworkElement = (FrameworkElement) content;
        if (frameworkElement?.DataContext is IViewLoaded viewLoaded)
          await viewLoaded.LoadedAsync(parameter);
      }
      else
      {
        var frameworkElement = (FrameworkElement) content;
        _tabControl.SelectedIndex = _tabControl.Items.Add(frameworkElement);
        if (frameworkElement?.DataContext is IViewInitialized viewInitialized && !viewInitialized.IsInitialized)
          await viewInitialized.InitializedAsync(parameter);

        if (frameworkElement?.DataContext is IViewLoaded viewLoaded)
          await viewLoaded.LoadedAsync(parameter);
      }
    }
  }
}