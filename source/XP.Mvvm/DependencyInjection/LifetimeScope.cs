using Grace.DependencyInjection;

namespace XP.Mvvm.DependencyInjection;

public class LifetimeScope
: ILifetimeScope
{
  private readonly IExportLocatorScope _scope;

  public LifetimeScope(IExportLocatorScope scope)
  {
    _scope = scope;
  }

  public T Get<T>()
  {
    return _scope.Locate<T>();
  }

  
  public T Get<T>(string key)
  {
    return _scope.Locate<T>(withKey: key);
  }

  public void Dispose()
  {
    _scope?.Dispose();
  }
}