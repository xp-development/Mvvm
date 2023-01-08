using System.Collections.ObjectModel;

namespace XP.Mvvm;

public class HierarchicalItem : MenuItemBase
{
  public ObservableCollection<MenuItemBase> Items { get; } = new();
}