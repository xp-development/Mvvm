using System.Threading.Tasks;

namespace XP.Mvvm;

public interface IViewUnloaded
{
  Task UnloadedAsync();
}