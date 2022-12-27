using System.Threading.Tasks;

namespace XP.Mvvm.Regions
{
  public interface IRegion
  {
    Task AttachAsync(object content, object parameter = null);
    Task CloseAsync(object content);
    Task CloseCurrentAsync();
    object Current { get; }
  }
}