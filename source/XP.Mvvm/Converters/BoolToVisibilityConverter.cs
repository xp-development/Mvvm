using System;
using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;

namespace XP.Mvvm.Converters
{
  public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
  {
    public bool VisibleValue { get; set; } = true;
    
    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      if (value is bool boolValue)
      {
        return boolValue == VisibleValue ? Visibility.Visible : Visibility.Collapsed;
      }

      return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}