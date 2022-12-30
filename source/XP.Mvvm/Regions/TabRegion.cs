using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using log4net;

namespace XP.Mvvm.Regions
{
  public class TabRegion : IRegion
  {
    private readonly TabView _tabControl;
    private object _attachParameter;

    private static readonly ILog _log = LogManager.GetLogger(typeof(TabRegion));

    public TabRegion(TabView tabControl)
    {
      _tabControl = tabControl;
    }

    public async Task AttachAsync(object content, object parameter = null)
    {
      _log.Debug($"Attach {content.GetType()}");

      if (await UnloadContent(_tabControl.SelectedItem))
        return;

      var firstOrDefault = _tabControl.TabItems.Cast<TabViewItem>().FirstOrDefault(x => x.Content == content);
      if (firstOrDefault != null)
      {
        _tabControl.SelectedItem = firstOrDefault;
        var frameworkElement = (FrameworkElement) content;
        if (frameworkElement.DataContext is IViewLoaded viewLoaded)
        {
          await viewLoaded.LoadedAsync(parameter);
          _log.Debug($"ViewLoaded {frameworkElement.GetType()}");
        }
      }
      else
      {
        var frameworkElement = (FrameworkElement)content;
        var tabViewItem = new TabViewItem{ Content = content, Header = (ViewModelBase)frameworkElement.DataContext };
        _tabControl.TabItems.Add(tabViewItem);
        _tabControl.SelectedItem = tabViewItem;

        if (frameworkElement.DataContext is IViewInitialized { IsInitialized: false } viewInitialized)
        {
          await viewInitialized.InitializedAsync(parameter);
          _log.Debug($"ViewInitialized {frameworkElement.GetType()}");
        }

        if (frameworkElement.DataContext is IViewLoaded)
        {
          _attachParameter = parameter;
          frameworkElement.Loaded += ViewLoaded;
        }
      }
    }

    private async void ViewLoaded(object sender, RoutedEventArgs e)
    {
      var attachParameter = _attachParameter;
      var frameworkElement = (FrameworkElement)sender;
      frameworkElement.Loaded -= ViewLoaded;
      var viewModel = (IViewLoaded)frameworkElement.DataContext;
      await viewModel.LoadedAsync(attachParameter);
      _log.Debug($"ViewLoaded {frameworkElement.GetType()}");
      _attachParameter = null;
    }

    private async Task<bool> UnloadContent(object content)
    {
      if (content == null)
        return false;

      var tabViewItem = content as TabViewItem;
      var tabContent = tabViewItem?.Content as FrameworkElement;
      if (tabContent?.DataContext is IViewUnloading viewUnloading)
      {
        var viewUnloadingEventArgs = new ViewUnloadingEventArgs();
        await viewUnloading.UnloadingAsync(viewUnloadingEventArgs);
        _log.Debug($"Unloading {tabContent.GetType()}");
        if (viewUnloadingEventArgs.Cancel)
        {
          _log.Debug($"Unloading {tabContent.GetType()} cancelled.");
          return true;
        }
      }

      if (tabContent?.DataContext is IViewUnloaded viewUnloaded)
      {
        await viewUnloaded.UnloadedAsync();
        _log.Debug($"Unloaded {tabContent.GetType()}");
      }

      return false;
    }

    public Task CloseCurrentAsync()
    {
      return CloseAsync(_tabControl.SelectedItem);
    }

    public async Task CloseAsync(object content)
    {
      _log.Debug($"Close {content.GetType()}");

      await UnloadContent(content);
      var tabViewItem = content as TabViewItem;
      var frameworkElement = tabViewItem?.Content as FrameworkElement;
      if (frameworkElement?.DataContext is IViewDeinitialized viewDeinitialized)
      {
        await viewDeinitialized.DeinitializedAsync();
        _log.Debug($"ViewDeinitialized {frameworkElement.GetType()}");
      }

      var tabControlSelectedContent = content;
      _tabControl.TabItems.Remove(tabControlSelectedContent);
    }

    public object Current => (_tabControl.SelectedItem as TabViewItem)?.Content;
  }
}