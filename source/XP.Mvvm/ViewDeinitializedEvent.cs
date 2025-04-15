using XP.Mvvm.Events;

namespace XP.Mvvm;

public class ViewDeinitializedEvent<TViewModel>
: IEvent
{
  public TViewModel ViewModel { get; set; }
}