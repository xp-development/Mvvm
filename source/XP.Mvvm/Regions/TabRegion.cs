using System.Collections.Generic;
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
      _tabControl.SelectionChanged += TabControlSelectionChanged;
    }

    private bool _suppressChanging;

    private async void TabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (_suppressChanging)
        return;

      var reselectItems = new List<object>();
      foreach (var removedItem in e.RemovedItems)
      {
        if (await UnloadContent(removedItem))
          reselectItems.Add(removedItem);
      }

      foreach (var reselectItem in reselectItems)
      {
        _suppressChanging = true;
        _tabControl.SelectedItem = reselectItem;
        _suppressChanging = false;
      }

      foreach (var addedItem in e.AddedItems)
      {
        var frameworkElement = (FrameworkElement)((TabViewItem)addedItem).Content;
        await ViewLoadedAsync(frameworkElement);
      }
    }

    private async Task ViewLoadedAsync(FrameworkElement frameworkElement)
    {
      if (frameworkElement.DataContext is IViewLoaded viewLoaded)
      {
        await viewLoaded.LoadedAsync(_attachParameter);
        _log.Debug($"ViewLoaded {frameworkElement.GetType()}");
        _attachParameter = null;
      }
    }

    public async Task AttachAsync(object content, object parameter = null)
    {
      _log.Debug($"Attach {content.GetType()}");
      
      var firstOrDefault = _tabControl.TabItems.Cast<TabViewItem>().FirstOrDefault(x => x.Content == content);
      if (firstOrDefault == null)
      {
        var frameworkElement = (FrameworkElement)content;
        var tabViewItem = new TabViewItem { Content = content, Header = (ViewModelBase)frameworkElement.DataContext };
        
        if (frameworkElement.DataContext is IViewInitialized { IsInitialized: false } viewInitialized)
        {
          await viewInitialized.InitializedAsync(parameter);
          _log.Debug($"ViewInitialized {frameworkElement.GetType()}");
        }

        // _suppressChanging = true;
        _attachParameter = parameter;
        _tabControl.TabItems.Add(tabViewItem);
        _tabControl.SelectedItem = tabViewItem;

        // if (frameworkElement.DataContext is IViewLoaded)
        // {
        //   _attachParameter = parameter;
        //   await ViewLoadedAsync(frameworkElement);
        // }

        // _suppressChanging = false;
      }
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

    public async Task ReplaceCurrentWithAsync(object content, object parameter = null)
    {
      _log.Debug($"Replace {_tabControl.SelectedItem.GetType()} with {content.GetType()}");

      _suppressChanging = true;
      await CloseAsync(_tabControl.SelectedItem);
      _suppressChanging = false;
      await AttachAsync(content, parameter);
    }

    public async Task CloseAsync(object content)
    {
      _log.Debug($"Close {content.GetType()}");

      if (await UnloadContent(content))
        return;

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