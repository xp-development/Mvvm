using System.Threading.Tasks;

namespace XP.Mvvm;

public abstract class SimpleViewModelBase : NotifyPropertyChangedBase
{
  private string _displayName;

  public string DisplayName
  {
    get => _displayName;
    set
    {
      _displayName = value;
      InvokePropertyChanged();
    }
  }
}