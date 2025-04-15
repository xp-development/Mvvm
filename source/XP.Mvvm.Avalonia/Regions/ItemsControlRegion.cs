using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using log4net;
using XP.Mvvm.Events;

namespace XP.Mvvm.Avalonia.Regions;

public class ItemsControlRegion : RegionBase
{
    private readonly ItemsControl _itemsControl;
    private static readonly ILog _log = LogManager.GetLogger(typeof(ItemsControlRegion));

    public ItemsControlRegion(IEventAggregator eventAggregator, ItemsControl itemsControl)
    : base(eventAggregator)
    {
        _itemsControl = itemsControl;
    }

    public override async Task AttachAsync(object content, object parameter = null)
    {
        _log.Debug($"Attach {content.GetType()}");

        _itemsControl.Items.Add(content);
        var frameworkElement = (Control)content;
        await PublishEventMessage(typeof(ViewInitializedEvent<>), frameworkElement.DataContext, parameter);

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

    public override async Task CloseAsync(object content)
    {
        _log.Debug($"Close {content.GetType()}");

        if (await UnloadContent(content))
            return;

        var frameworkElement = content as Control;
        await PublishEventMessage(typeof(ViewDeinitializedEvent<>), frameworkElement.DataContext, null);
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

    public override Task CloseCurrentAsync()
    {
        throw new NotSupportedException();
    }

    public override Task ReplaceCurrentWithAsync(object content, object parameter = null)
    {
        throw new NotSupportedException();
    }

    public override object Current => throw new NotSupportedException();
}