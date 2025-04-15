using System;
using System.Threading.Tasks;
using XP.Mvvm.Events;
using XP.Mvvm.Regions;

namespace XP.Mvvm.Avalonia.Regions;

public abstract class RegionBase(IEventAggregator eventAggregator) : IRegion
{
  public abstract Task AttachAsync(object content, object parameter = null);

  public abstract Task CloseAsync(object content);

  public abstract Task CloseCurrentAsync();

  public abstract Task ReplaceCurrentWithAsync(object content, object parameter = null);

  public abstract object Current { get; }

  protected async Task PublishEventMessage(Type viewEventType, object dataContext, object parameter)
  {
    var eventType = viewEventType.MakeGenericType(dataContext.GetType());
    var instance = Activator.CreateInstance(eventType);
    eventType.GetProperty(nameof(ViewInitializedEvent<object>.ViewModel))!.SetValue(instance, dataContext);
    eventType.GetProperty(nameof(ViewInitializedEvent<object>.Parameter))?.SetValue(instance, parameter);
    await eventAggregator.PublishAsync((dynamic)instance);
  }
}