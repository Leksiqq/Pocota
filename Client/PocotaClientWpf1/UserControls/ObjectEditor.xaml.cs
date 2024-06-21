using Net.Leksi.WpfMarkup;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static Net.Leksi.Pocota.Client.Constants;

namespace Net.Leksi.Pocota.Client.UserControls;

public partial class ObjectEditor : UserControl, INotifyPropertyChanged, IValueConverter
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? CurrentInputChanged;

    private const int s_HeaderWidthTreshold = 10;
    private const int s_ValueWidthParameter = 15;
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    private readonly Localizer _localizer;
    private IInputElement? _currentInput = null;
    public static readonly DependencyProperty ServiceProviderCatcherProperty = DependencyProperty.Register(
       nameof(ServiceProviderCatcher), typeof(XamlServiceProviderCatcher),
       typeof(ObjectEditor)
    );
    public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
       nameof(Target), typeof(object),
       typeof(ObjectEditor)
    );
    public CollectionViewSource PropertiesViewSource { get; private init; } = new();
    public XamlServiceProviderCatcher ServiceProviderCatcher 
    { 
        get => (XamlServiceProviderCatcher)GetValue(ServiceProviderCatcherProperty); 
        set => SetValue(ServiceProviderCatcherProperty, value);
    }
    public object? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }
    public Window Window { get; private set; } = null!;
    public bool? IsInsertMode {  get; private set; }
    public IInputElement? CurrentInput 
    { 
        get => _currentInput; 
        internal set
        {
            _currentInput = value;
            CurrentInputChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public ObjectEditor()
    {
        _localizer = (Application.Current.Resources[LocalizerResourceKey] as Localizer)!;
        InitializeComponent();
        Loaded += ObjectEditor_Loaded;
    }
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if("InsertInputMode".Equals(parameter))
        {
            if(value is bool b)
            {
                return b ? _localizer.Insert : _localizer.Overwrite;
            }
            else
            {
                return string.Empty;
            }
        }
        return value;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
    public void CalcColumnsWidth()
    {
        if ((Window?.IsActive ?? false) && Visibility is Visibility.Visible && PropertyNameColumn.ActualWidth > s_HeaderWidthTreshold) 
        {
            ScrollViewer scrollViewer = GetVisualDescendants(PropertiesView).OfType<ScrollViewer>().First();
            if (scrollViewer.ActualWidth - PropertyNameColumn.ActualWidth - s_ValueWidthParameter > 0)
            {
                PropertyValueColumn.Width = scrollViewer.ActualWidth - PropertyNameColumn.ActualWidth - s_ValueWidthParameter;
            }
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }
    }
    private void PropertiesView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged)
        {
            CalcColumnsWidth();
        }
    }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {

        if (e.Property.OwnerType == GetType())
        {
            if(e.Property == TargetProperty)
            {
                if(e.NewValue is null)
                {
                    PropertiesViewSource.Source = null;
                }
            }
            SetTemplateSelector();
        }
        base.OnPropertyChanged(e);
    }
    private void SetTemplateSelector()
    {
        if (ServiceProviderCatcher is { } && Target is { } && Window is { })
        {
            if(PropertyValueColumn.CellTemplateSelector is null)
            {
                string spName = $"sp{Guid.NewGuid()}";
                ParameterizedResourceExtension pre = new("PropertyTemplateSelector")
                {
                    Replaces = new string[] { $"$serviceProviderCatcher:{spName}" },
                };
                this.Window.Resources.Add(spName, ServiceProviderCatcher);
                PropertyValueColumn.CellTemplateSelector = pre.ProvideValue(ServiceProviderCatcher.ServiceProvider!) as DataTemplateSelector;
                this.Window.Resources.Remove(spName);
            }
            PropertiesViewSource.Source = Target.GetType().GetProperties().Select(p => new Field { PropertyName = p.Name, Target = Target });
        }
    }
    private static IEnumerable<DependencyObject> GetVisualDescendants(DependencyObject obj)
    {
        int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
        for (int i = 0; i < childrenCount; ++i)
        {
            DependencyObject child = VisualTreeHelper.GetChild(obj, i);
            yield return child;
            foreach (var descendant in GetVisualDescendants(child))
            {
                yield return descendant;
            }
        }
    }
    private void CheckColumnWidth()
    {
        if (PropertyNameColumn.ActualWidth > s_HeaderWidthTreshold)
        {
            Dispatcher.Invoke(CalcColumnsWidth);
        }
        else
        {
            Task.Delay(1).ContinueWith(t => Task.Run(CheckColumnWidth));
        }
    }
    private void ObjectEditor_Loaded(object sender, RoutedEventArgs e)
    {
        for (DependencyObject dop = this; dop is { }; dop = VisualTreeHelper.GetParent(dop))
        {
            if (dop is Window window)
            {
                Window = window;
                SetTemplateSelector();
                break;
            }
        }
        CheckColumnWidth();
    }
}
