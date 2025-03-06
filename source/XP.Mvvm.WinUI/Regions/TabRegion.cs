using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using XP.Mvvm.Regions;

namespace XP.Mvvm.WinUI.Regions
{
  public class TabRegion : IRegion
  {
    private readonly TabView _tabControl;
    private bool _suppressChanging;
    private static readonly ILog _log = LogManager.GetLogger(typeof(TabRegion));
    private TaskCompletionSource _taskCompletionSource;

    public TabRegion(TabView tabControl)
    {
      _tabControl = tabControl;
      _tabControl.SelectionChanged += TabControlSelectionChanged;
    }

    public async Task AttachAsync(object content, object parameter = null)
    {
      _log.Debug($"Attach {content.GetType()}");
      
      var firstOrDefault = _tabControl.TabItems.Cast<TabViewItem>().FirstOrDefault(x => x.Content == content);
      if (firstOrDefault != null)
        return;
      
      var frameworkElement = (FrameworkElement)content;
      _taskCompletionSource = new TaskCompletionSource();
      var tabViewItem = new TabViewItem { Content = content, Header = (ViewModelBase)frameworkElement.DataContext, Tag = parameter };
      _tabControl.TabItems.Add(tabViewItem);
      _tabControl.SelectedItem = tabViewItem;
      await _taskCompletionSource.Task;
      _taskCompletionSource = null;
    }
    
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
        var tabViewItem = (TabViewItem)addedItem;
        var frameworkElement = (FrameworkElement)tabViewItem.Content;
        await LoadContentAsync(frameworkElement, tabViewItem.Tag);
      }
      
      _taskCompletionSource?.SetResult();
    }

    private async Task LoadContentAsync(FrameworkElement frameworkElement, object parameter)
    {
      var controlsToLoad = new List<FrameworkElement> { frameworkElement };
      controlsToLoad.AddRange(FindVisualChilds(frameworkElement, x => x.GetValue(RegionManager.RegionProperty) != null));
      foreach (var element in controlsToLoad.Where(x => x != null)
                                            .Select(x => x.DataContext)
                                            .Distinct())
      {
        if (element is not IViewInitialized { IsInitialized: false } viewInitialized)
          continue;
        
        await viewInitialized.InitializedAsync(parameter);
        _log.Debug($"ViewInitialized {element.GetType()}");
      }

      foreach (var element in controlsToLoad.Where(x => x != null)
                                            .Select(x => x.DataContext)
                                            .Distinct())
      {
        if (element is not IViewLoading viewLoading)
          continue;
        
        await viewLoading.LoadingAsync(parameter);
        _log.Debug($"ViewLoading {element.GetType()}");
      }

      foreach (var element in controlsToLoad.Where(x => x != null)
                                            .Select(x => x.DataContext)
                                            .Distinct())
      {
        if (element is not IViewLoaded viewLoaded)
          continue;
        
        await viewLoaded.LoadedAsync(parameter);
        _log.Debug($"ViewLoaded {element.GetType()}");
      }
    }

    private async Task<bool> UnloadContent(object content)
    {
      if (content == null)
        return false;

      var tabViewItem = content as TabViewItem;
      var tabContent = tabViewItem?.Content as FrameworkElement;
      var controlsToUnload = new List<FrameworkElement> { tabContent };
      controlsToUnload.AddRange(FindVisualChilds(tabContent, x => x.GetValue(RegionManager.RegionProperty) != null));
      
      var viewUnloadingEventArgs = new ViewUnloadingEventArgs();
      var viewModelsToUnload = GetViewModelsToUnload(controlsToUnload);
      foreach (var frameworkElement in viewModelsToUnload)
      {
        if (frameworkElement is not IViewUnloading viewUnloading)
          continue;
        
        await viewUnloading.UnloadingAsync(viewUnloadingEventArgs);
        _log.Debug($"Unloading {frameworkElement.GetType()}");
        if (viewUnloadingEventArgs.Cancel)
        {
          _log.Debug($"Unloading {frameworkElement.GetType()} cancelled.");
          return true;
        }
      }

      foreach (var frameworkElement in viewModelsToUnload)
      {
        if (frameworkElement is IViewUnloaded viewUnloaded)
        {
          await viewUnloaded.UnloadedAsync();
          _log.Debug($"Unloaded {tabContent.GetType()}");
        }
      }

      return false;
    }

    private static List<object> GetViewModelsToUnload(List<FrameworkElement> controlsToUnload)
    {
      var list = controlsToUnload.Where(x => x != null)
                                 .Select(x => x.DataContext).ToList();
      list.AddRange(controlsToUnload.Where(x => x != null && x is ContentControl)
                                    .Select(x => (((ContentControl)x).Content as FrameworkElement)?.DataContext));
      var controlsToUnloadList = list.Distinct().ToList();
      return controlsToUnloadList;
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
      var controlsToDeinitialize = new List<FrameworkElement> { frameworkElement };
      controlsToDeinitialize.AddRange(FindVisualChilds(frameworkElement, x => x.GetValue(RegionManager.RegionProperty) != null));
      var viewModelsToUnload = GetViewModelsToUnload(controlsToDeinitialize);
      foreach (var element in viewModelsToUnload)
      {
        if (element is not IViewDeinitialized viewDeinitialized)
          continue;

        await viewDeinitialized.DeinitializedAsync();
        _log.Debug($"ViewDeinitialized {element.GetType()}");
      }

      var tabControlSelectedContent = content;
      _tabControl.TabItems.Remove(tabControlSelectedContent);
    }

    public object Current => (_tabControl.SelectedItem as TabViewItem)?.Content;

    private static IEnumerable<FrameworkElement> FindVisualChilds(FrameworkElement dependencyObject, Func<FrameworkElement, bool> condition)
    {
      if (dependencyObject == null)
        yield return (FrameworkElement)Enumerable.Empty<FrameworkElement>();
      
      for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
      {
        var child = VisualTreeHelper.GetChild(dependencyObject, i) as FrameworkElement;
        if (child == null)
          continue;

        if (child is FrameworkElement visualChild && condition(visualChild))
        {
          if (visualChild is ItemsControl itemsControl)
          {
            foreach (var item in itemsControl.Items)
              yield return (FrameworkElement)item;
          }
          else if (visualChild is TabView tabView)
          {
            foreach (var tabItem in tabView.TabItems.Cast<TabViewItem>())
              yield return (FrameworkElement)tabItem.Content;              
          }
          else if(visualChild is ContentControl)
            yield return visualChild;
        }
        
        foreach (var childOfChild in FindVisualChilds(child, condition))
          yield return childOfChild;
      }
    }
  }
}