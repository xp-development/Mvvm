using System;
using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;

namespace XP.Mvvm.Converters
{
  public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
  {
    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      if (value is bool boolValue)
      {
        return boolValue ? Visibility.Visible : Visibility.Collapsed;
      }

      return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}