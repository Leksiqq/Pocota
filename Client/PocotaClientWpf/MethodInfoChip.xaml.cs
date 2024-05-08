using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
namespace Net.Leksi.Pocota.Client;
public partial class MethodInfoChip : UserControl, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public static readonly DependencyProperty MethodInfoProperty = DependencyProperty.Register("MethodInfo", typeof(MethodInfo), typeof(MethodInfoChip));
    public static readonly DependencyProperty TypeChipStyleProperty = DependencyProperty.Register("TypeChipStyle", typeof(Style), typeof(MethodInfoChip));
    public MethodInfo MethodInfo
    {
        get => (MethodInfo)GetValue(MethodInfoProperty);
        set => SetValue(MethodInfoProperty, value);
    }
    public Style TypeChipStyle
    {
        get => (Style)GetValue(TypeChipStyleProperty);
        set => SetValue(TypeChipStyleProperty, value);
    }
    public Type? ReturnType => MethodInfo?.ReturnType;
    public Type? DeclaringType => MethodInfo?.DeclaringType;
    public string? MethodName => MethodInfo?.Name;
    public Visibility ArgumentsVisibility => (MethodInfo?.GetParameters().Length > 0) ? Visibility.Visible : Visibility.Collapsed;
    public MethodInfoChip()
    {
        InitializeComponent();
    }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == MethodInfoProperty || e.Property == TypeChipStyleProperty)
        {
            if(e.Property == MethodInfoProperty)
            {
                Arguments.Children.Clear();
                bool first = true;
                foreach (ParameterInfo para in MethodInfo.GetParameters())
                {
                    if (!first)
                    {
                        Arguments.Children.Add(new TextBlock(new Run(", ")));
                    }
                    else
                    {
                        first = false;
                    }
                    Arguments.Children.Add(new TypeChip { Type = para.ParameterType, Style = TypeChipStyle });
                    Arguments.Children.Add(new TextBlock(new Run($" {para.Name}")));
                }
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
        base.OnPropertyChanged(e);
    }
}
