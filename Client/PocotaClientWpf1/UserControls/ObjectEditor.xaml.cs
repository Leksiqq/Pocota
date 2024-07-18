using Net.Leksi.Util;
using Net.Leksi.WpfMarkup;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

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
    private bool _columnsWidthCalculated = false;
    private bool _isFirstLoad = true;
    public static readonly DependencyProperty ServiceProviderProperty = DependencyProperty.Register(
       nameof(ServiceProvider), typeof(IServiceProvider),
       typeof(ObjectEditor)
    );
    public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
       nameof(Target), typeof(object),
       typeof(ObjectEditor)
    );
    public static readonly DependencyProperty PropertiesViewSourceProperty = DependencyProperty.Register(
       nameof(PropertiesViewSource), typeof(CollectionViewSource),
       typeof(ObjectEditor)
    );
    public CollectionViewSource PropertiesViewSource
    {
        get => (CollectionViewSource)GetValue(PropertiesViewSourceProperty);
        set => SetValue(PropertiesViewSourceProperty, value);
    }
    public IServiceProvider ServiceProvider 
    { 
        get => (IServiceProvider)GetValue(ServiceProviderProperty); 
        set => SetValue(ServiceProviderProperty, value);
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
        PropertiesViewSource = new CollectionViewSource();
        _localizer = Application.Current.GetLocalizer();
        InitializeComponent();
        Loaded += ObjectEditor_Loaded;
        if (Application.Current.TryFindResource("LifetimeObserver") is LifetimeObserver lto)
        {
            lto.TraceObject(this);
        }
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
            _columnsWidthCalculated = true;
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
        if (ServiceProvider != null && Target != null && Window != null)
        {
            if(PropertyValueColumn.CellTemplateSelector == null)
            {
                string spName = $"sp{Guid.NewGuid()}";
                ParameterizedResourceExtension pre = new("PropertyTemplateSelector")
                {
                    Replaces = new string[] { $"$serviceProviderCatcher:{spName}" },
                };
                this.Window.Resources.Add(spName, ServiceProvider);
                PropertyValueColumn.CellTemplateSelector = pre.ProvideValue(ServiceProvider) as DataTemplateSelector;
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
        bool windowIsLoaded = false;
        double propertyNameColumnActualWidth = 0;
        Dispatcher.Invoke(() => 
        {
            windowIsLoaded = Window?.IsLoaded ?? false;
            propertyNameColumnActualWidth = PropertyNameColumn.ActualWidth;
        });
        if (propertyNameColumnActualWidth > s_HeaderWidthTreshold)
        {
            Dispatcher.Invoke(CalcColumnsWidth);
        }
        else if(windowIsLoaded && !_columnsWidthCalculated)
        {
            Task.Delay(1).ContinueWith(t => Task.Run(CheckColumnWidth));
        }
    }
    private void ObjectEditor_Loaded(object sender, RoutedEventArgs e)
    {
        if (_isFirstLoad)
        {
            _isFirstLoad = false;
            for (DependencyObject dop = this; dop != null; dop = VisualTreeHelper.GetParent(dop))
            {
                if (dop is Window window)
                {
                    Window = window;
                    var oe = GetVisualDescendants(window).OfType<ObjectEditor>().FirstOrDefault();
                    if(oe == this)
                    {
                        SetTemplateSelector();
                        CheckColumnWidth();
                    }
                    break;
                }
            }
        }
    }
}
