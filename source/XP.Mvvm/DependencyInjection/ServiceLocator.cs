using System;
using System.Collections.Generic;
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

    public IEnumerable<T> GetAll<T>()
    {
      return _container.LocateAll<T>();
    }

    public T Get<T>(string key)
    {
      return _container.Locate<T>(withKey: key);
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

    public void RegisterSingletonObject<TInterface, TInterface2, TObject>()
      where TObject : TInterface, TInterface2
    {
      _container.Configure(c => c.Export<TObject>().As<TInterface>().Lifestyle.Singleton().As<TInterface2>().Lifestyle.Singleton());
    }

    public void RegisterSingletonObject<TInterface>(TInterface obj)
    {
      _container.Configure(c => c.ExportInstance(obj).Lifestyle.Singleton());
    }

    public void RegisterTransientObject<TInterface>(TInterface obj)
    {
      _container.Configure(c => c.ExportInstance(obj).As<TInterface>());
    }

    public void RegisterTransientObject<TInterface, TObject>()
      where TObject : TInterface
    {
      _container.Configure(c => c.Export<TObject>().As<TInterface>());
    }

    public void RegisterTransientObject<TInterface, TObject>(string key)
      where TObject : TInterface
    {
      _container.Configure(c => c.Export<TObject>().AsKeyed<TInterface>(key));
    }
  }
}