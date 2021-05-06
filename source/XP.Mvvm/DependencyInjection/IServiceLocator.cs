using System;

namespace XP.Mvvm.DependencyInjection
{
  public interface IServiceLocator
  {
    T Get<T>();
    object Get(Type type);
    IServiceLocator CreateChildServiceLocator();

    void RegisterSingletonObject<TInterface, TObject>()
      where TObject : TInterface;

    void RegisterSingletonObject<TInterface>(TInterface obj);

    void RegisterTransientObject<TInterface, TObject>()
      where TObject : TInterface;
  }
}