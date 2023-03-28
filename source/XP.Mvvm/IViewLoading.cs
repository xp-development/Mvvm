using System.Threading.Tasks;

namespace XP.Mvvm;

public interface IViewLoading
{
    Task LoadingAsync(object parameter = null);
}