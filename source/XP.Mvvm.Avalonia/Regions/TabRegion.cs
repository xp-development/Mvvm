using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using log4net;
using XP.Mvvm.Regions;

namespace XP.Mvvm.Avalonia.Regions
{
  public class TabRegion : IRegion
  {
    private readonly TabControl _tabControl;
    private bool _suppressChanging;
    private static readonly ILog _log = LogManager.GetLogger(typeof(TabRegion));
    private TaskCompletionSource _taskCompletionSource;

    public TabRegion(TabControl tabControl)
    {
      _tabControl = tabControl;
      _tabControl.SelectionChanged += TabControlSelectionChanged;
    }

    public async Task AttachAsync(object content, object parameter = null)
    {
      _log.Debug($"Attach {content.GetType()}");
      
      var firstOrDefault = _tabControl.Items.Cast<TabItem>().FirstOrDefault(x => x.Content == content);
      if (firstOrDefault != null)
        return;
      
      var frameworkElement = (Control)content;
      _taskCompletionSource = new TaskCompletionSource();
      var tabViewItem = new TabItem
                        {
                          Content = content,
                          Header = frameworkElement.DataContext,
                          Tag = parameter
                        };
      _tabControl.Items.Add(tabViewItem);
      _tabControl.SelectedItem = tabViewItem;
      await _taskCompletionSource.Task;
      _taskCompletionSource = null;
    }
    
    private async void TabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (sender != e.Source)
        return;
      
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
        var tabViewItem = (TabItem)addedItem;
        var frameworkElement = (Control)tabViewItem.Content;
        await LoadContentAsync(frameworkElement, tabViewItem.Tag);
      }
      
      _taskCompletionSource?.SetResult();
    }

    private async Task LoadContentAsync(Control frameworkElement, object parameter)
    {
      var controlsToLoad = new List<Control> { frameworkElement };
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

      var tabViewItem = content as TabItem;
      var tabContent = tabViewItem?.Content as Control;
      var controlsToUnload = new List<Control> { tabContent };
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

    private static List<object> GetViewModelsToUnload(List<Control> controlsToUnload)
    {
      var list = controlsToUnload.Where(x => x != null)
                                 .Select(x => x.DataContext).ToList();
      list.AddRange(controlsToUnload.Where(x => x != null && x is ContentControl)
                                    .Select(x => (((ContentControl)x).Content as Control)?.DataContext));
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

      var tabViewItem = content as TabItem;
      var frameworkElement = tabViewItem?.Content as Control;
      var controlsToDeinitialize = new List<Control> { frameworkElement };
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
      _tabControl.Items.Remove(tabControlSelectedContent);
    }

    public object Current => (_tabControl.SelectedItem as TabItem)?.Content;

    private static IEnumerable<Control> FindVisualChilds(ILogical control, Func<Control, bool> condition)
    {
      if (control == null)
        yield break;
      
      foreach(var child in control.GetLogicalChildren())
      {
        if (child is Control visualChild && condition(visualChild))
        {
          if (visualChild is TabControl tabView)
          {
            foreach (var tabItem in tabView.Items.Cast<TabItem>())
              yield return (Control)tabItem.Content;              
          }
          else if (visualChild is ItemsControl itemsControl)
          {
            foreach (var item in itemsControl.GetLogicalChildren())
              yield return (Control)item;
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