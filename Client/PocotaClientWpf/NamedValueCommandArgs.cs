using System.Windows;

namespace Net.Leksi.Pocota.Client;

public class NamedValueCommandArgs: Freezable
{
    public static readonly DependencyProperty NamedValueProperty = DependencyProperty.Register(
       nameof(NamedValue), typeof(NamedValue),
       typeof(NamedValueCommandArgs)
    );
    public static readonly DependencyProperty ActionProperty = DependencyProperty.Register(
       nameof(Action), typeof(NamedValueAction),
       typeof(NamedValueCommandArgs)
    );
    public NamedValue? NamedValue
    {
        get => (NamedValue)GetValue(NamedValueProperty);
        set => SetValue(NamedValueProperty, value);
    }
    public NamedValueAction Action
    {
        get => (NamedValueAction)GetValue(ActionProperty);
        set => SetValue(ActionProperty, value);
    }
    protected override Freezable CreateInstanceCore()
    {
        return this;
    }

}
