using System;
using System.Collections.ObjectModel;

namespace XP.Mvvm
{
  public class MenuItem
  {
    public string Title { get; set; }
    public DelegateCommand Command { get; set; }
    public MenuItem Parent { get; set; }
    public int Order { get; set; }
    public ObservableCollection<MenuItem> Items { get; } = new();

    public bool IsCheckable { get; set; }
    public bool IsChecked { get; set; }
  }
}