using System.Threading.Tasks;

namespace XP.Mvvm;

public interface IViewInitialized
{
  bool IsInitialized { get; }
  Task InitializedAsync(object parameter = null);
}