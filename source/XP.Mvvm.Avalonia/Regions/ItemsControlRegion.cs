using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using log4net;
using XP.Mvvm.Events;
using XP.Mvvm.Regions;

namespace XP.Mvvm.Avalonia.Regions;

public class ItemsControlRegion(ItemsControl itemsControl, IEventAggregator eventAggregator) : IRegion
{
  public async Task AttachAsync(object content, object parameter = null)
  {
    if (itemsControl.Items.Contains(content))
    {
      Log.Debug($"{content.GetType()} is already attached.");
      return;
    }
    
    Log.Debug($"Attach {content.GetType()}");
    itemsControl.Items.Add(content);
    var frameworkElement = (Control)content;
    var initializeState = frameworkElement.DataContext as IViewInitializeState;
    if (initializeState?.IsInitialized == false)
    {
      await eventAggregator.PublishAsync(new InitializingEvent(parameter, initializeState));
      if (frameworkElement.DataContext is IViewInitializing { IsInitialized: false } viewInitializing)
      {
        await viewInitializing.InitializingAsync(parameter);
        Log.Debug($"ViewInitializing {frameworkElement.GetType()}");
      }

      if (frameworkElement.DataContext is IViewInitialized { IsInitialized: false } viewInitialized)
      {
        await viewInitialized.InitializedAsync(parameter);
        Log.Debug($"ViewInitialized {frameworkElement.GetType()}");
      }

      await eventAggregator.PublishAsync(new InitializedEvent(parameter, frameworkElement.DataContext));
      initializeState.IsInitialized = true;
    }

    await eventAggregator.PublishAsync(new LoadingEvent(parameter, frameworkElement.DataContext));
    if (frameworkElement.DataContext is IViewLoading viewLoading)
    {
      await viewLoading.LoadingAsync(parameter);
      Log.Debug($"ViewLoading {frameworkElement.GetType()}");
    }

    await eventAggregator.PublishAsync(new LoadedEvent(parameter, frameworkElement.DataContext));
    if (frameworkElement.DataContext is IViewLoaded viewLoaded)
    {
      await viewLoaded.LoadedAsync(parameter);
      Log.Debug($"ViewLoaded {frameworkElement.GetType()}");
    }
  }

  public async Task CloseAsync(object content)
  {
    Log.Debug($"Close {content.GetType()}");

    if (await UnloadContent(content))
      return;

    var frameworkElement = content as Control;
    await eventAggregator.PublishAsync(new DeinitializedEvent(frameworkElement?.DataContext));
    if (frameworkElement?.DataContext is IViewDeinitialized viewDeinitialized)
    {
      await viewDeinitialized.DeinitializedAsync();
      Log.Debug($"ViewDeinitialized {frameworkElement.GetType()}");
    }

    itemsControl.Items.Remove(content);
  }

  public Task CloseCurrentAsync()
  {
    throw new NotSupportedException();
  }

  public Task ReplaceCurrentWithAsync(object content, object parameter = null)
  {
    throw new NotSupportedException();
  }

  public object Current => throw new NotSupportedException();

  private async Task<bool> UnloadContent(object content)
  {
    if (content == null)
      return false;

    var frameworkElement = content as Control;
    var viewUnloadingEventArgs = new ViewUnloadingEventArgs();
    await eventAggregator.PublishAsync(new UnloadingEvent(viewUnloadingEventArgs, frameworkElement.DataContext));
    if (frameworkElement?.DataContext is IViewUnloading viewUnloading)
    {
      await viewUnloading.UnloadingAsync(viewUnloadingEventArgs);
      Log.Debug($"Unloading {frameworkElement.GetType()}");
    }
    
    if (viewUnloadingEventArgs.Cancel)
    {
      Log.Debug($"Unloading {frameworkElement.GetType()} cancelled.");
      return true;
    }

    await eventAggregator.PublishAsync(new UnloadedEvent(frameworkElement?.DataContext));
    if (frameworkElement?.DataContext is IViewUnloaded viewUnloaded)
    {
      await viewUnloaded.UnloadedAsync();
      Log.Debug($"Unloaded {frameworkElement.GetType()}");
    }

    return false;
  }

  private static readonly ILog Log = LogManager.GetLogger(typeof(ItemsControlRegion));
}