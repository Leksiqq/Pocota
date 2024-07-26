using Net.Leksi.WpfMarkup;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace Net.Leksi.Pocota.Client.UserControls;
public partial class ObjectEditor : UserControl, IValueConverter
{
    public event EventHandler? CurrentInputChanged;
    private const int s_HeaderWidthTreshold = 10;
    private const int s_ValueWidthParameter = 15;
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
    public static readonly DependencyProperty WindowProperty = DependencyProperty.Register(
       nameof(Window), typeof(Window),
       typeof(ObjectEditor)
    );
    private readonly Localizer _localizer;
    private bool _columnsWidthCalculated = false;
    private bool _templateSelectorIsSet = false;
    private IInputElement? _currentInput = null;
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
    public Window Window
    {
        get => (Window)GetValue(WindowProperty);
        set => SetValue(WindowProperty, value);
    }
    public bool? IsInsertMode { get; private set; }
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
        _localizer = Application.Current.GetLocalizer();
        PropertiesViewSource = new CollectionViewSource();
        InitializeComponent();
        NotifyInstanceCreated.Notify(this);
    }
    public void Clear()
    {
        PropertyValueColumn.CellTemplateSelector = null;
    }
    public void CalcColumnsWidth()
    {
        if ((Window?.IsActive ?? false) && Visibility == Visibility.Visible && PropertyNameColumn.ActualWidth > s_HeaderWidthTreshold)
        {
            ScrollViewer scrollViewer = Utilities.GetVisualDescendants(PropertiesView).OfType<ScrollViewer>().First();
            if (scrollViewer.ActualWidth - PropertyNameColumn.ActualWidth - s_ValueWidthParameter > 0)
            {
                PropertyValueColumn.Width = scrollViewer.ActualWidth - PropertyNameColumn.ActualWidth - s_ValueWidthParameter;
            }
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _columnsWidthCalculated = true;
        }
    }
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if ("InsertInputMode".Equals(parameter))
        {
            if (value is bool b)
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
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {

        if (e.Property.OwnerType == GetType())
        {
            if (e.Property == TargetProperty)
            {
                if (e.NewValue == null)
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
        if (ServiceProvider != null && Target != null && Window != null && !_templateSelectorIsSet)
        {
            _templateSelectorIsSet = true;
            string spName = $"sp{Guid.NewGuid()}";
            ParameterizedResourceExtension pre = new("PropertyTemplateSelector")
            {
                Replaces = new string[] { $"$serviceProviderCatcher:{spName}" },
            };
            Window.Resources.Add(spName, ServiceProvider);
            PropertyValueColumn.CellTemplateSelector = pre.ProvideValue(ServiceProvider) as DataTemplateSelector;
            Window.Resources.Remove(spName);
            PropertiesViewSource.Source = Target.GetType().GetProperties().Select(p => new Field { PropertyName = p.Name, Target = Target });
            CheckColumnWidth();
        }
    }
    private void PropertiesView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged)
        {
            CalcColumnsWidth();
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
        if (windowIsLoaded && propertyNameColumnActualWidth > s_HeaderWidthTreshold)
        {
            Dispatcher.Invoke(CalcColumnsWidth);
        }
        else if (!_columnsWidthCalculated)
        {
            Task.Delay(1).ContinueWith(t => Task.Run(CheckColumnWidth));
        }
    }
}
