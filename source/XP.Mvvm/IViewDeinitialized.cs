using System.Threading.Tasks;

namespace XP.Mvvm;

public interface IViewDeinitialized
{
  Task DeinitializedAsync();
}