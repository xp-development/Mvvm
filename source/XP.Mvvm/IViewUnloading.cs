using System.Threading.Tasks;

namespace XP.Mvvm
{
  public interface IViewUnloading
  {
    Task UnloadingAsync(ViewUnloadingEventArgs eventArgs);
  }
}