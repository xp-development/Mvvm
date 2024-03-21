using System;

namespace XP.Mvvm.DependencyInjection;

public interface ILifetimeScope : IDisposable
{
  T Get<T>();

  T Get<T>(string key);
}