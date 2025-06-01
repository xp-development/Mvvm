using System;

namespace XP.Mvvm.DependencyInjection
{
  public interface IServiceLocator
  {
    T Get<T>();
    T Get<T>(object key);
    object Get(Type type);
    IServiceLocator CreateChildServiceLocator();

    void RegisterSingletonObject<TInterface, TObject>()
      where TObject : TInterface;

    void RegisterSingletonObject<TInterface>(TInterface obj);

    void RegisterTransientObject<TInterface, TObject>()
      where TObject : TInterface;

    void RegisterSingletonObject(Type interfaceType, Type implementationType);

    public void RegisterSingletonPerScopeObject<TInterface, TObject>(object key)
    where TObject : TInterface;

    void RegisterSingletonObject<TInterface, TInterface2, TObject>()
    where TObject : TInterface, TInterface2;

    void RegisterTransientObject<TInterface, TObject>(object key)
    where TObject : TInterface;

    void RegisterSingletonObject<TInterface, TObject>(object key)
    where TObject : TInterface;

    ILifetimeScope BeginLifetimeScope();
  }
}