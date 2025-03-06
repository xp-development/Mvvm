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

    public static readonly StyledProperty<Type> ViewModelTypeProperty =
      AvaloniaProperty.RegisterAttached<Control, Type>("ViewModelType", typeof(ViewModelLocator), typeof(Type), false, BindingMode.TwoWay);

    public static IServiceLocator ViewModelServiceLocator;

  }
}