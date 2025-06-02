using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XP.Mvvm;

public class DelegateCommand : DelegateCommand<object, object>
{
  public DelegateCommand(Func<object, Task> executeMethod, Func<object, bool> canExecuteMethod = null)
  : base(executeMethod, canExecuteMethod)
  {
  }
}

public class DelegateCommand<TExecuteArg, TCanExecuteArg> : ICommand
{
  private readonly Func<TCanExecuteArg, bool> _canExecuteMethod;
  private readonly Func<TExecuteArg, Task> _executeMethod;

  public DelegateCommand(Func<TExecuteArg, Task> executeMethod, Func<TCanExecuteArg, bool> canExecuteMethod = null)
  {
    _executeMethod = executeMethod ?? throw new ArgumentNullException(nameof(executeMethod));
    _canExecuteMethod = canExecuteMethod;
  }

  public event EventHandler CanExecuteChanged;

  bool ICommand.CanExecute(object parameter)
  {
    return CanExecute((TCanExecuteArg) parameter);
  }

  async void ICommand.Execute(object parameter)
  {
    await Execute((TExecuteArg) parameter);
  }

  public Task Execute(TExecuteArg arg)
  {
    return _executeMethod(arg);
  }

  public bool CanExecute(TCanExecuteArg arg)
  {
    return _canExecuteMethod == null || _canExecuteMethod(arg);
  }

  public void InvokeCanExecuteChanged()
  {
    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
  }
}