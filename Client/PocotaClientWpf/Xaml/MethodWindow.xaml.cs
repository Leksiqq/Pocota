using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using static Net.Leksi.Pocota.Client.Constants;

namespace Net.Leksi.Pocota.Client
{
    /// <summary>
    /// Логика взаимодействия для MethodWindow.xaml
    /// </summary>
    public partial class MethodWindow : Window, IWindowLauncher
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly IServiceProvider _services;
        private readonly PropertyChangedEventArgs _propertyChangedEventArgs = new(null);
        public ObservableCollection<Property> Parameters { get; private init; } = [];
        public WindowsList Windows { get; private init; }
        public string MethodName => ConnectorMethod.Method is { } 
            ? $"{Util.BuildTypeName(ConnectorMethod.Method.DeclaringType!)}.{ConnectorMethod.Method.Name}" 
            : string.Empty;
        public EditWindowLauncher Launcher {get; private init;}
        public bool KeysOnly { get => true; set { } }
        public string ServiceKey { get; set; }
        private ConnectorMethod ConnectorMethod { get; init; }
        public MethodWindow(ConnectorMethod connectorMethod)
        {
            ConnectorMethod = connectorMethod;
            ServiceKey = ConnectorMethod.Connector.ServiceKey;
            _services = (IServiceProvider)Application.Current.Resources[ServiceProvider];
            Windows = _services.GetRequiredService<WindowsList>();
            Launcher = new EditWindowLauncher($"{MethodName}(...):", this);
            InitializeComponent();
            Metrics.MethodInfo = ConnectorMethod.Method;
            foreach(ParameterInfo parameter in ConnectorMethod.Method.GetParameters())
            {
                if(parameter.ParameterType != typeof(CancellationToken) && parameter.Name != "target")
                {
                    Parameters.Add(Property.Create(parameter)!);
                }
            }
            CalcColumnsWidth(ParametersView.ActualWidth);
            Windows.Touch();
        }
        protected override void OnClosed(EventArgs e)
        {
            Windows.Touch();
            Application.Current.MainWindow?.Activate();
            base.OnClosed(e);
        }
        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                CalcColumnsWidth(e.NewSize.Width);
            }
        }
        private void CalcColumnsWidth(double width)
        {
            ParameterValueColumn.Width = width * 0.89 - ParameterNameColumn.ActualWidth;
        }
        private void ListViewItem_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalcColumnsWidth(ParametersView.ActualWidth);
        }
    }
}
