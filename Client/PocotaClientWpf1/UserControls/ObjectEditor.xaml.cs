using Net.Leksi.WpfMarkup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
    private readonly Localizer _localizer;
    private IInputElement? _currentInput = null;
    public static readonly DependencyProperty ServiceProviderCatcherProperty = DependencyProperty.Register(
       nameof(ServiceProviderCatcher), typeof(XamlServiceProviderCatcher),
       typeof(ObjectEditor)
    );
    public static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register(
       nameof(Properties), typeof(ObservableCollection<Property>),
       typeof(ObjectEditor)
    );
    public static readonly DependencyProperty WindowProperty = DependencyProperty.Register(
       nameof(Window), typeof(Window),
       typeof(ObjectEditor)
    );
    public CollectionViewSource PropertiesViewSource { get; private init; } = new();
    public XamlServiceProviderCatcher ServiceProviderCatcher 
    { 
        get => (XamlServiceProviderCatcher)GetValue(ServiceProviderCatcherProperty); 
        set => SetValue(ServiceProviderCatcherProperty, value);
    }
    public ObservableCollection<Property> Properties
    {
        get => (ObservableCollection<Property>)GetValue(PropertiesProperty);
        set => SetValue(PropertiesProperty, value);
    }
    public Window Window
    {
        get => (Window)GetValue(WindowProperty);
        set => SetValue(WindowProperty, value);
    }
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
        if ((Window?.IsActive ?? false) && Visibility is Visibility.Visible) 
        {
            ScrollViewer scrollViewer = GetVisualDescendants(PropertiesView).OfType<ScrollViewer>().First();
            if(scrollViewer.ActualWidth - PropertyNameColumn.ActualWidth - 15 > 0)
            {
                PropertyValueColumn.Width = scrollViewer.ActualWidth - PropertyNameColumn.ActualWidth - 15;
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
            if(e.Property == PropertiesProperty)
            {
                if (e.OldValue is ObservableCollection<Property> oc)
                {
                    oc.CollectionChanged -= Properties_CollectionChanged;
                }
                if (e.NewValue is ObservableCollection<Property> oc1)
                {
                    oc1.CollectionChanged += Properties_CollectionChanged;
                }
            }
            else if(e.Property == WindowProperty)
            {
                if (e.OldValue is Window oldWindow)
                {
                    oldWindow.Activated -= Window_Activated;
                }
                if (e.NewValue is Window newWindow)
                {
                    newWindow.Activated += Window_Activated;
                }
            }
            SetTemplateSelector();
        }
        base.OnPropertyChanged(e);
    }

    private void Window_Activated(object? sender, EventArgs e)
    {
        CalcColumnsWidth();
    }

    private void Properties_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CalcColumnsWidth();
    }
    private void SetTemplateSelector()
    {
        if (ServiceProviderCatcher is { } && Properties is { } && Window is { } && PropertyValueColumn.CellTemplateSelector is null)
        {
            string spName = $"sp{Guid.NewGuid()}";
            ParameterizedResourceExtension pre = new("PropertyTemplateSelector")
            {
                Replaces = new string[] { $"$serviceProviderCatcher:{spName}" },
            };
            this.Window.Resources.Add(spName, ServiceProviderCatcher);
            PropertyValueColumn.CellTemplateSelector = pre.ProvideValue(ServiceProviderCatcher.ServiceProvider!) as DataTemplateSelector;
            this.Window.Resources.Remove(spName);
            PropertiesViewSource.Source = Properties;
        }
    }
    private void oe_Loaded(object sender, RoutedEventArgs e)
    {
        CalcColumnsWidth();
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
    private static IEnumerable<DependencyObject> GetLogicalDescendants(DependencyObject obj)
    {
        foreach (var child in LogicalTreeHelper.GetChildren(obj))
        {
            if(child is DependencyObject dob)
            {
                yield return dob;
                foreach (var descendant in GetLogicalDescendants(dob))
                {
                    yield return descendant;
                }
            }
        }
    }
}
