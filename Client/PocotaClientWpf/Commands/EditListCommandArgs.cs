using System.Windows;

namespace Net.Leksi.Pocota.Client;

public class EditListCommandArgs : Freezable
{
    public static readonly DependencyProperty ActionProperty = DependencyProperty.Register(
       nameof(Action), typeof(PropertyAction),
       typeof(EditListCommandArgs)
    );
    public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(
       nameof(Item), typeof(object),
       typeof(EditListCommandArgs)
    );
    public object? Item
    {
        get => GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }
    public PropertyAction Action
    {
        get => (PropertyAction)GetValue(ActionProperty);
        set => SetValue(ActionProperty, value);
    }
    protected override Freezable CreateInstanceCore()
    {
        return this;
    }
}
