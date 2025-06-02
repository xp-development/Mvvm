namespace XP.Mvvm;

public interface IViewLocator
{
  T GetView<T>();
}