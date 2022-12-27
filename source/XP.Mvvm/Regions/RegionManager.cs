using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace XP.Mvvm.Regions
{
  public class RegionManager : IRegionManager
  {
    private static readonly Dictionary<string, IRegion> _regions = new();

    public static readonly DependencyProperty RegionProperty = DependencyProperty.RegisterAttached(
      "Region", typeof(string), typeof(RegionManager), new PropertyMetadata(default(string), RegionChangedCallback));

    private static void RegionChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is TabView tabControl)
        _regions[(string)e.NewValue] = new TabRegion(tabControl);
      else
        _regions[(string)e.NewValue] = new SingleContentRegion((ContentControl) d);
    }

    public static void SetRegion(DependencyObject element, string value)
    {
      element.SetValue(RegionProperty, value);
    }

    public static string GetRegion(DependencyObject element)
    {
      return (string) element.GetValue(RegionProperty);
    }

    public IRegion GetRegion(string region)
    {
      return _regions[region];
    }
  }
}