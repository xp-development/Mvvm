using System;
using System.Windows;

namespace XP.Mvvm
{
  public class ViewModelLocator
  {
    public static readonly DependencyProperty ViewModelTypeProperty =
      DependencyProperty.RegisterAttached("ViewModelType", typeof(Type), typeof(ViewModelLocator),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, ViewModelTypeChanged));

    public static Func<Type, object> ViewModelFunc;

    private static void ViewModelTypeChanged(DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
      ((FrameworkElement) dependencyObject).DataContext =
        ViewModelFunc((Type) dependencyPropertyChangedEventArgs.NewValue);
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