using System;

namespace XP.Mvvm.DependencyInjection
{
  public interface IServiceLocator
  {
    T Get<T>();
    T Get<T>(string key);
    object Get(Type type);
    IServiceLocator CreateChildServiceLocator();

    void RegisterSingletonObject<TInterface, TObject>()
      where TObject : TInterface;

    void RegisterSingletonObject(Type interfaceType, Type implementationType);

    void RegisterSingletonObject<TInterface>(TInterface obj);

    void RegisterTransientObject<TInterface, TObject>()
      where TObject : TInterface;

    ILifetimeScope BeginLifetimeScope();
  }
}