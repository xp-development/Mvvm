using XP.Mvvm.DependencyInjection;

namespace XP.Mvvm
{
  public class ViewLocator : IViewLocator
  {
    private readonly IServiceLocator _serviceLocator;

    public ViewLocator(IServiceLocator serviceLocator)
    {
      _serviceLocator = serviceLocator;
    }

    public T GetView<T>()
    {
      var viewModelServiceLocator = ViewModelLocator.ViewModelServiceLocator;
      ViewModelLocator.ViewModelServiceLocator = _serviceLocator;
      var view = _serviceLocator.Get<T>();
      ViewModelLocator.ViewModelServiceLocator = viewModelServiceLocator;
      return view;

    }
  }
}