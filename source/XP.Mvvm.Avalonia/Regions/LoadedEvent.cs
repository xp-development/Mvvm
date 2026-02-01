using XP.Mvvm.Events;

namespace XP.Mvvm.Avalonia.Regions;

public class LoadedEvent(object parameter, object dataContext) : IEvent
{
  public object Parameter { get; } = parameter;
  public object DataContext { get; set; } = dataContext;
}