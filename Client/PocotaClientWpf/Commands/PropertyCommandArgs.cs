using System.Windows;

namespace Net.Leksi.Pocota.Client;

public class PropertyCommandArgs: Freezable
{
    public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(
       nameof(Property), typeof(Property),
       typeof(PropertyCommandArgs)
    );
    public static readonly DependencyProperty ActionProperty = DependencyProperty.Register(
       nameof(Action), typeof(PropertyAction),
       typeof(PropertyCommandArgs)
    );
    public static readonly DependencyProperty LauncherProperty = DependencyProperty.Register(
       nameof(Launcher), typeof(Window),
       typeof(PropertyCommandArgs)
    );
    public Property? Property
    {
        get => (Property)GetValue(PropertyProperty);
        set => SetValue(PropertyProperty, value);
    }
    public PropertyAction Action
    {
        get => (PropertyAction)GetValue(ActionProperty);
        set => SetValue(ActionProperty, value);
    }
    public Window? Launcher
    {
        get => (Window)GetValue(LauncherProperty);
        set => SetValue(LauncherProperty, value);
    }
    public string? AltName { get; set; } = null;
    public PropertyCommandArgs()
    {
        Launcher = null;
    }
    protected override Freezable CreateInstanceCore()
    {
        return this;
    }

}
