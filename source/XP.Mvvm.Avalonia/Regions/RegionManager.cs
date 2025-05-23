﻿using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using XP.Mvvm.Regions;

namespace XP.Mvvm.Avalonia.Regions
{
  public class RegionManager : AvaloniaObject, IRegionManager
  {
    static RegionManager()
    {
      RegionProperty.Changed.AddClassHandler<Control>(RegionPropertyChanged);
    }

    private static void RegionPropertyChanged(Control control, AvaloniaPropertyChangedEventArgs arg)
    {
      if (control is TabControl tabControl)
        _regions[(string)arg.NewValue] = new TabRegion(tabControl);
      else if (control is ItemsControl itemsControl)
        _regions[(string)arg.NewValue] = new ItemsControlRegion(itemsControl);
      else
        _regions[(string)arg.NewValue] = new SingleContentRegion((ContentControl) control);
    }

    private static readonly Dictionary<string, IRegion> _regions = new();

    public static readonly StyledProperty<string> RegionProperty = AvaloniaProperty.RegisterAttached<Control, string>(
      "Region", typeof(RegionManager));
    
    public static void SetRegion(AvaloniaObject element, string commandValue)
    {
      element.SetValue(RegionProperty, commandValue);
    }
    
    public static string GetRegion(AvaloniaObject element)
    {
      return element.GetValue(RegionProperty);
    }

    public IRegion GetRegion(string regionName)
    {
      return _regions[regionName];
    }
  }
}