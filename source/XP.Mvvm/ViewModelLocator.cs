using System;
using System.Windows;
using Xamarin.Forms;
using XP.Mvvm.DependencyInjection;

namespace XP.Mvvm
{
  public class ViewModelLocator
  {
    public static readonly BindableProperty ViewModelTypeProperty =
      BindableProperty.CreateAttached("ViewModelType", typeof(Type), typeof(ViewModelLocator),
        null, propertyChanged: ViewModelTypeChanged);

    private static void ViewModelTypeChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
      bindable.BindingContext = ViewModelServiceLocator.Get((Type) newvalue);
    }

    public static IServiceLocator ViewModelServiceLocator;

    public static void SetViewModelType(BindableObject dependencyObject, Type value)
    {
      dependencyObject.SetValue(ViewModelTypeProperty, value);
    }

    public static Type GetViewModelType(BindableObject dependencyObject)
    {
      return (Type) dependencyObject.GetValue(ViewModelTypeProperty);
    }
  }
}