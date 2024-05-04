using System.Windows;
using System.Windows.Controls;

namespace Net.Leksi.Pocota.Client;

public class SortByColumnArgs : Freezable
{
    public static readonly DependencyProperty FieldNameProperty = DependencyProperty.Register(
       "FieldName", typeof(string),
       typeof(SortByColumnArgs)
    );
    public static readonly DependencyProperty ButtonProperty = DependencyProperty.Register(
       "Button", typeof(Button),
       typeof(SortByColumnArgs)
    );
    public string? FieldName
    {
        get => (string)GetValue(FieldNameProperty);
        set => SetValue(FieldNameProperty, value);
    }
    public Button? Button
    {
        get => (Button)GetValue(ButtonProperty);
        set => SetValue(ButtonProperty, value);
    }
    protected override Freezable CreateInstanceCore()
    {
        return this;
    }
}
