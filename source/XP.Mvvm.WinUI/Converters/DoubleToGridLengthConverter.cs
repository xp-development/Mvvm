using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;

namespace XP.Mvvm.WinUI.Converters;

public class DoubleToGridLengthConverter : MarkupExtension, IValueConverter
{
    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
    {
        return this;
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double doubleValue)
        {
            return new GridLength(doubleValue);
        }

        return new GridLength(1, GridUnitType.Auto);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is GridLength gridLengthValue)
        {
            return gridLengthValue.Value;
        }

        return 1d;
    }
}