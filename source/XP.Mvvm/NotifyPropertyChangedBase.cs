using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XP.Mvvm
{
  public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected void InvokePropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}