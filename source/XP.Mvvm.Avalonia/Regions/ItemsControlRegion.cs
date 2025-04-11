using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using log4net;
using XP.Mvvm.Regions;

namespace XP.Mvvm.Avalonia.Regions;

public class ItemsControlRegion : IRegion
{
    private readonly ItemsControl _itemsControl;
    private static readonly ILog _log = LogManager.GetLogger(typeof(ItemsControlRegion));

    public ItemsControlRegion(ItemsControl itemsControl)
    {
        _itemsControl = itemsControl;
    }

    public async Task AttachAsync(object content, object parameter = null)
    {
        _log.Debug($"Attach {content.GetType()}");

        _itemsControl.Items.Add(content);
        var frameworkElement = (Control)content;
        if (frameworkElement.DataContext is IViewInitialized { IsInitialized: false } viewInitialized)
        {
            await viewInitialized.InitializedAsync(parameter);
            _log.Debug($"ViewInitialized {frameworkElement.GetType()}");
        }
        
        if (frameworkElement.DataContext is IViewLoading viewLoading)
        {
            await viewLoading.LoadingAsync(parameter);
            _log.Debug($"ViewLoading {frameworkElement.GetType()}");
        }
        
        if (frameworkElement.DataContext is IViewLoaded viewLoaded)
        {
            await viewLoaded.LoadedAsync(parameter);
            _log.Debug($"ViewLoaded {frameworkElement.GetType()}");
        }
    }

    public async Task CloseAsync(object content)
    {
        _log.Debug($"Close {content.GetType()}");

        if (await UnloadContent(content))
            return;

        var frameworkElement = content as Control;
        if (frameworkElement?.DataContext is IViewDeinitialized viewDeinitialized)
        {
            await viewDeinitialized.DeinitializedAsync();
            _log.Debug($"ViewDeinitialized {frameworkElement.GetType()}");
        }
        
        _itemsControl.Items.Remove(content);
    }
    
    private async Task<bool> UnloadContent(object content)
    {
        if (content == null)
            return false;

        var frameworkElement = content as Control;
        if (frameworkElement?.DataContext is IViewUnloading viewUnloading)
        {
            var viewUnloadingEventArgs = new ViewUnloadingEventArgs();
            await viewUnloading.UnloadingAsync(viewUnloadingEventArgs);
            _log.Debug($"Unloading {frameworkElement.GetType()}");
            if (viewUnloadingEventArgs.Cancel)
            {
                _log.Debug($"Unloading {frameworkElement.GetType()} cancelled.");
                return true;
            }
        }

        if (frameworkElement?.DataContext is IViewUnloaded viewUnloaded)
        {
            await viewUnloaded.UnloadedAsync();
            _log.Debug($"Unloaded {frameworkElement.GetType()}");
        }

        return false;
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
}