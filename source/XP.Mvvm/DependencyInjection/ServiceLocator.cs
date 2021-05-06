using System;
using Grace.DependencyInjection;

namespace XP.Mvvm.DependencyInjection
{
  public class ServiceLocator : IServiceLocator
  {
    private readonly IInjectionScope _container;

    public ServiceLocator(IInjectionScope container)
    {
      _container = container;
      RegisterSingletonObject<IServiceLocator>(this);
    }

    public T Get<T>()
    {
      return _container.Locate<T>();
    }

    public object Get(Type type)
    {
      return _container.Locate(type);
    }

    public IServiceLocator CreateChildServiceLocator()
    {
      return new ServiceLocator(_container.CreateChildScope());
    }

    public void RegisterSingletonObject<TInterface, TObject>()
      where TObject : TInterface
    {
      _container.Configure(c => c.Export<TObject>().As<TInterface>().Lifestyle.Singleton());
    }

    public void RegisterSingletonObject<TInterface>(TInterface obj)
    {
      _container.Configure(c => c.ExportInstance(obj).Lifestyle.Singleton());
    }

    public void RegisterTransientObject<TInterface, TObject>()
      where TObject : TInterface
    {
      _container.Configure(c => c.Export<TObject>().As<TInterface>());
    }
  }
}