using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;

namespace XP.Mvvm.Converters;

public class BoolToGridLengthConverter : MarkupExtension, IValueConverter
{
  public GridLength TrueValue { get; set; } = new();
  public GridLength FalseValue { get; set; } = new();
    
  protected override object ProvideValue(IXamlServiceProvider serviceProvider)
  {
    return this;
  }

  public object Convert(object value, Type targetType, object parameter, string language)
  {
    if (value is bool boolValue)
    {
      return boolValue ? TrueValue : FalseValue;
    }

    return Visibility.Visible;
  }

  public object ConvertBack(object value, Type targetType, object parameter, string language)
  {
    throw new NotImplementedException();
  }
}