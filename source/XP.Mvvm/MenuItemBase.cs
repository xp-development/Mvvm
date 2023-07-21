namespace XP.Mvvm
{
  public abstract class MenuItemBase
  {
    public string Title { get; set; }
    
    public HierarchicalItem Parent { get; set; }
    public int Order { get; set; }
  }
}