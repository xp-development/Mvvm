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
    Log.Debug($"Attach {content.GetType()}");

    itemsControl.Items.Add(content);
    var frameworkElement = (Control)content;
    if (frameworkElement.DataContext is IViewInitializing { IsInitialized: false } viewInitializing)
    {
      await viewInitializing.InitializingAsync(parameter);
      await eventAggregator.PublishAsync(new InitializingEvent(parameter, viewInitializing));
      Log.Debug($"ViewInitializing {frameworkElement.GetType()}");
    }

    if (frameworkElement.DataContext is IViewInitialized { IsInitialized: false } viewInitialized)
    {
      await viewInitialized.InitializedAsync(parameter);
      await eventAggregator.PublishAsync(new InitializedEvent(parameter, viewInitialized));
      Log.Debug($"ViewInitialized {frameworkElement.GetType()}");
    }

    if (frameworkElement.DataContext is IViewLoading viewLoading)
    {
      await viewLoading.LoadingAsync(parameter);
      await eventAggregator.PublishAsync(new LoadingEvent(parameter, viewLoading));
      Log.Debug($"ViewLoading {frameworkElement.GetType()}");
    }

    if (frameworkElement.DataContext is IViewLoaded viewLoaded)
    {
      await viewLoaded.LoadedAsync(parameter);
      await eventAggregator.PublishAsync(new LoadedEvent(parameter, viewLoaded));
      Log.Debug($"ViewLoaded {frameworkElement.GetType()}");
    }
  }

  public async Task CloseAsync(object content)
  {
    Log.Debug($"Close {content.GetType()}");

    if (await UnloadContent(content))
      return;

    var frameworkElement = content as Control;
    if (frameworkElement?.DataContext is IViewDeinitialized viewDeinitialized)
    {
      await viewDeinitialized.DeinitializedAsync();
      await eventAggregator.PublishAsync(new DeinitializedEvent(viewDeinitialized));
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
    if (frameworkElement?.DataContext is IViewUnloading viewUnloading)
    {
      var viewUnloadingEventArgs = new ViewUnloadingEventArgs();
      await viewUnloading.UnloadingAsync(viewUnloadingEventArgs);
      await eventAggregator.PublishAsync(new UnloadingEvent(viewUnloadingEventArgs, viewUnloading));
      Log.Debug($"Unloading {frameworkElement.GetType()}");
      if (viewUnloadingEventArgs.Cancel)
      {
        Log.Debug($"Unloading {frameworkElement.GetType()} cancelled.");
        return true;
      }
    }

    if (frameworkElement?.DataContext is IViewUnloaded viewUnloaded)
    {
      await viewUnloaded.UnloadedAsync();
      await eventAggregator.PublishAsync(new UnloadedEvent(viewUnloaded));
      Log.Debug($"Unloaded {frameworkElement.GetType()}");
    }

    return false;
  }

  private static readonly ILog Log = LogManager.GetLogger(typeof(ItemsControlRegion));
}