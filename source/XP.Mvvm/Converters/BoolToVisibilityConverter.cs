using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace XP.Mvvm.Converters
{
  public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool boolValue)
      {
        return boolValue ? Visibility.Visible : Visibility.Collapsed;
      }

      return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}