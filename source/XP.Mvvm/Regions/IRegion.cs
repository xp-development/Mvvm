using System.Threading.Tasks;

namespace XP.Mvvm.Regions;

public interface IRegion
{
  Task AttachAsync(object content, object parameter = null);
  Task CloseAsync(object content);
  Task CloseCurrentAsync();
  Task ReplaceCurrentWithAsync(object content, object parameter = null);
  object Current { get; }
}