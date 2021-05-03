using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace XP.Mvvm.Regions
{
  public class RegionManager : IRegionManager
  {
    private static readonly Dictionary<string, IRegion> _regions = new Dictionary<string, IRegion>();

    public static readonly DependencyProperty RegionProperty = DependencyProperty.RegisterAttached(
      "Region", typeof(string), typeof(RegionManager), new PropertyMetadata(default(string), RegionChangedCallback));

    private static void RegionChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      _regions.Add((string)e.NewValue, new SingleContentRegion((ContentControl) d));
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