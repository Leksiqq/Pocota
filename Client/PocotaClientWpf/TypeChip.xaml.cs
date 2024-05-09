using Net.Leksi.WpfMarkup;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
namespace Net.Leksi.Pocota.Client;
public partial class TypeChip : UserControl, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private const int s_dueTime = 300;
    private bool _mouseOverPopup = false;
    private readonly Timer _timer;

    public static readonly DependencyProperty ExpandCaptionProperty = DependencyProperty.Register("ExpandCaption", typeof(string), typeof(TypeChip));
    public static readonly DependencyProperty CollapseCaptionProperty = DependencyProperty.Register("CollapseCaption", typeof(string), typeof(TypeChip));
    public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(TypeChip));
    public string? ExpandCaption
    {
        get => (string)GetValue(ExpandCaptionProperty);
        set => SetValue(ExpandCaptionProperty, value);
    }
    public string? CollapseCaption
    {
        get => (string)GetValue(CollapseCaptionProperty);
        set => SetValue(CollapseCaptionProperty, value);
    }
    public Type Type
    {
        get => (Type)GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }
    public string TypeName
    {
        get 
        {
            if(Type is { })
            {
                if (!Type.IsGenericType)
                {
                    return Type.Name;
                }
                return Type.GetGenericTypeDefinition().Name[..Type.GetGenericTypeDefinition().Name.IndexOf('`')];
            }
            return string.Empty;
        }
    }
    public string? Namespace
    {
        get
        {
            if (Type is { })
            {
                return Type.Namespace;
            }
            return string.Empty;
        }
    }
    public Visibility GenericArgumentsVisibility => Type is { } && Type.IsGenericType ? Visibility.Visible : Visibility.Collapsed;
    public Visibility NamespaceVisibility => Type is { } && !string.IsNullOrEmpty(Type.Namespace) && IsCaptionExpanded ? Visibility.Visible : Visibility.Collapsed;
    public bool IsCaptionExpanded { get; set; } = false;
    public TypeChip()
    {
        _timer = new Timer(CallBack);
        InitializeComponent();
    }
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        IsCaptionExpanded = !IsCaptionExpanded;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        TypeNamePopup.IsOpen = false;
    }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if(e.Property.OwnerType == typeof(TypeChip))
        {
            if (e.Property == TypeProperty)
            {
                GenericArguments.Children.Clear();
                if (Type.IsGenericType)
                {
                    bool first = true;
                    foreach (Type arg in Type.GetGenericArguments())
                    {
                        if (!first)
                        {
                            GenericArguments.Children.Add(new TextBlock(new Run(", ")));
                        }
                        else
                        {
                            first = false;
                        }
                        GenericArguments.Children.Add(new TypeChip { Type = arg, Style = this.Style });
                    }
                }
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
        else if(e.Property == StyleProperty)
        {
            if(Type?.IsGenericType ?? false)
            {
                foreach (var child in GenericArguments.Children)
                {
                    if(child is TypeChip tc)
                    {
                        tc.Style = (Style)e.NewValue;
                    }
                }
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
        base.OnPropertyChanged(e);
    }
    private void TypeNameCaption_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        TypeNamePopup.IsOpen = true;
    }
    private void TypeNameCaption_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        _timer.Change(s_dueTime, Timeout.Infinite);
    }
    private void TypeNamePopup_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        _mouseOverPopup = true;
    }
    private void TypeNamePopup_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        _mouseOverPopup = false;
        TypeNamePopup.IsOpen = false;
    }
    private void CallBack(object? state)
    {
        if (!_mouseOverPopup)
        {
            Dispatcher.Invoke(() => TypeNamePopup.IsOpen = false);
        }
    }
}
