using System;
using System.Collections.Generic;
using Grace.DependencyInjection;
using log4net;

namespace XP.Mvvm.DependencyInjection
{
  public class ServiceLocator : IServiceLocator
  {
    private readonly IInjectionScope _container;
    private readonly ILog _log = LogManager.GetLogger(typeof(ServiceLocator));

    public ServiceLocator(IInjectionScope container)
    {
      _container = container;
      RegisterSingletonObject<IServiceLocator>(this);
    }

    public T Get<T>()
    {
      _log.Debug($"Get {typeof(T)}");
      return _container.Locate<T>();
    }

    public IEnumerable<T> GetAll<T>()
    {
      _log.Debug($"GetAll {typeof(T)}");
      return _container.LocateAll<T>();
    }

    public T Get<T>(string key)
    {
      _log.Debug($"Get {typeof(T)} with key: {key}");
      return _container.Locate<T>(withKey: key);
    }

    public object Get(Type type)
    {
      _log.Debug($"Get {type}");
      return _container.Locate(type);
    }

    public IServiceLocator CreateChildServiceLocator()
    {
      return new ServiceLocator(_container.CreateChildScope());
    }

    public void RegisterSingletonObject<TInterface, TObject>()
      where TObject : TInterface
    {
      _log.Debug($"RegisterSingletonObject export {typeof(TObject)} as {typeof(TInterface)}");
      _container.Configure(c => c.Export<TObject>().As<TInterface>().Lifestyle.Singleton());
    }

    public void RegisterSingletonObject<TInterface, TInterface2, TObject>()
      where TObject : TInterface, TInterface2
    {
      _log.Debug($"RegisterSingletonObject export {typeof(TObject)} as {typeof(TInterface)} and as {typeof(TInterface2)}");
      _container.Configure(c => c.Export<TObject>().As<TInterface>().Lifestyle.Singleton().As<TInterface2>().Lifestyle.Singleton());
    }

    public void RegisterSingletonObject<TInterface>(TInterface obj)
    {
      _log.Debug($"RegisterSingletonObject export {obj.GetType()} as {typeof(TInterface)}");
      _container.Configure(c => c.ExportInstance(obj).Lifestyle.Singleton());
    }

    public void RegisterTransientObject<TInterface>(TInterface obj)
    {
      _log.Debug($"RegisterTransientObject export {obj.GetType()} as {typeof(TInterface)}");
      _container.Configure(c => c.ExportInstance(obj).As<TInterface>());
    }

    public void RegisterTransientObject<TInterface, TObject>()
      where TObject : TInterface
    {
      _log.Debug($"RegisterTransientObject export {typeof(TObject)} as {typeof(TInterface)}");
      _container.Configure(c => c.Export<TObject>().As<TInterface>());
    }

    public void RegisterTransientObject<TInterface, TObject>(string key)
      where TObject : TInterface
    {
      _log.Debug($"RegisterTransientObject export {typeof(TObject)} as {typeof(TInterface)} with key {key}");
      _container.Configure(c => c.Export<TObject>().AsKeyed<TInterface>(key));
    }
  }
}