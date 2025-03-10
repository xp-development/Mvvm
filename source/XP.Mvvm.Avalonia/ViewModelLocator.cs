using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using XP.Mvvm.DependencyInjection;

namespace XP.Mvvm.Avalonia
{
  public class ViewModelLocator : AvaloniaObject
  {
    static ViewModelLocator()
    {
      ViewModelTypeProperty.Changed.AddClassHandler<Control>(ViewModelTypePropertyChanged);
    }

    private static void ViewModelTypePropertyChanged(Control control, AvaloniaPropertyChangedEventArgs arg2)
    {
      control.DataContext = ViewModelServiceLocator.Get((Type)arg2.NewValue);
    }

    public static readonly AttachedProperty<Type> ViewModelTypeProperty =
      AvaloniaProperty.RegisterAttached<ViewModelLocator, Control, Type>("ViewModelType", typeof(Type), false, BindingMode.TwoWay);
    
    public static void SetViewModelType(AvaloniaObject element, Type commandValue)
    {
      element.SetValue(ViewModelTypeProperty, commandValue);
    }

    public static Type GetViewModelType(AvaloniaObject element)
    {
      return element.GetValue(ViewModelTypeProperty);
    }
    
    public static IServiceLocator ViewModelServiceLocator;

  }
}