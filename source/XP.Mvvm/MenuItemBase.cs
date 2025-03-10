using System;
using System.Collections.ObjectModel;

namespace XP.Mvvm
{
  public class MenuItemBase
  {
    public string Title { get; set; }
    public DelegateCommand Command { get; set; }
    public Func<bool> IsCheckedFunc { get; set; }

    public HierarchicalItem Parent { get; set; }
    public int Order { get; set; }
    public ObservableCollection<MenuItemBase> Items { get; } = new();

  }
}