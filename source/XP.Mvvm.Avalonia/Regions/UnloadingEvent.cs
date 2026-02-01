using XP.Mvvm.Events;

namespace XP.Mvvm.Avalonia.Regions;

public class UnloadingEvent(ViewUnloadingEventArgs viewUnloadingEventArgs, object dataContext) : IEvent
{
  public object DataContext { get; set; } = dataContext;
  public ViewUnloadingEventArgs ViewUnloadingEventArgs { get; set; } = viewUnloadingEventArgs;
}