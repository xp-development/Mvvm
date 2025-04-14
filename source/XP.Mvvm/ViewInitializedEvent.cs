using XP.Mvvm.Events;

namespace XP.Mvvm;

public class ViewInitializedEvent<TViewModel>
: IEvent
{
  public TViewModel ViewModel { get; set; }

  public object Parameter { get; set; }
}