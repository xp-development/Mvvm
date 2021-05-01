using System.Threading.Tasks;

namespace XP.Mvvm
{
  public interface IViewLoaded
  {
    Task LoadedAsync(object parameter = null);
  }
}