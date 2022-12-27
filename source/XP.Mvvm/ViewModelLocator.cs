using System;
using Microsoft.UI.Xaml;
using XP.Mvvm.DependencyInjection;

namespace XP.Mvvm
{
  public class ViewModelLocator
  {
    public static readonly DependencyProperty ViewModelTypeProperty =
      DependencyProperty.RegisterAttached("ViewModelType", typeof(Type), typeof(ViewModelLocator), new PropertyMetadata(null, ViewModelTypeChanged));

    public static IServiceLocator ViewModelServiceLocator;

    private static void ViewModelTypeChanged(DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
      ((FrameworkElement) dependencyObject).DataContext = ViewModelServiceLocator.Get((Type) dependencyPropertyChangedEventArgs.NewValue);
    }

    public static void SetViewModelType(DependencyObject dependencyObject, Type value)
    {
      dependencyObject.SetValue(ViewModelTypeProperty, value);
    }

    public static Type GetViewModelType(DependencyObject dependencyObject)
    {
      return (Type) dependencyObject.GetValue(ViewModelTypeProperty);
    }
  }
}