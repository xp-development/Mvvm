using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace XP.Mvvm.Avalonia.Converters;

public class GridSizeConverter : MarkupExtension, IValueConverter
{ 
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    return new GridLength((double) value!, GridUnitType.Pixel);
  }

  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    return ((GridLength) value!).Value;
  }

  public override object ProvideValue(IServiceProvider serviceProvider)
  {
    return this;
  }
}