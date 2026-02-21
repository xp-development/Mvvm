using System.Threading.Tasks;

namespace XP.Mvvm;

public interface IViewInitializing
: IViewInitializeState
{
  Task InitializingAsync(object parameter = null);
}