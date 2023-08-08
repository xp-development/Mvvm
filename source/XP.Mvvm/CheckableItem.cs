using System;

namespace XP.Mvvm;

public class CheckableItem : MenuItemBase
{
  public DelegateCommand Command { get; set; }

  public Func<bool> IsCheckedFunc { get; set; }
}