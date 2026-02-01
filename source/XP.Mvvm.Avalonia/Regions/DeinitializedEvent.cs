using XP.Mvvm.Events;

namespace XP.Mvvm.Avalonia.Regions;

public class DeinitializedEvent(object dataContext) : IEvent
{
  public object DataContext { get; set; } = dataContext;
}