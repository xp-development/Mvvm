using System.Threading.Tasks;

namespace XP.Mvvm;

public interface IViewInitializing
{
  bool IsInitialized { get; }
  Task InitializingAsync(object parameter = null);
}