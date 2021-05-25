using System.Collections.Generic;
using Xamarin.Forms;

namespace XP.Mvvm.Regions
{
  public class RegionManager : IRegionManager
  {
    private static readonly Dictionary<string, IRegion> _regions = new Dictionary<string, IRegion>();

    public static readonly BindableProperty RegionProperty = BindableProperty.CreateAttached(
      "Region", typeof(string), typeof(RegionManager), default(string), propertyChanged: RegionChangedCallback);

    private static void RegionChangedCallback(BindableObject bindable, object oldvalue, object newvalue)
    {
      if (bindable is TabbedPage tabbedPage)
        _regions.Add((string)newvalue, new TabRegion(tabbedPage));
      else
        _regions.Add((string)newvalue, new SingleContentRegion((ContentView) bindable));
    }

    public static void SetRegion(BindableObject element, string value)
    {
      element.SetValue(RegionProperty, value);
    }

    public static string GetRegion(BindableObject element)
    {
      return (string) element.GetValue(RegionProperty);
    }

    public IRegion GetRegion(string region)
    {
      return _regions[region];
    }
  }
}