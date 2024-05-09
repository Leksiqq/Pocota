using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace Net.Leksi.Pocota.Client
{
    /// <summary>
    /// Логика взаимодействия для MethodWindow.xaml
    /// </summary>
    public partial class MethodWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly MethodInfo _method;
        private readonly IServiceProvider _services;
        public ObservableCollection<MethodParameter> Parameters { get; private init; } = [];
        public WindowsList Windows { get; private init; }
        public string MethodName => _method is { } ? $"{Util.BuildTypeName(_method.DeclaringType!)}.{_method.Name}" : string.Empty;
        public MethodWindow(MethodInfo method)
        {
            _method = method;
            _services = (IServiceProvider)Application.Current.Resources["ServiceProvider"];
            Windows = _services.GetRequiredService<WindowsList>();
            InitializeComponent();
            Metrics.MethodInfo = _method;
            foreach(ParameterInfo parameter in _method.GetParameters())
            {
                if(parameter.ParameterType != typeof(CancellationToken) && parameter.Name != "target")
                {
                    Parameters.Add(new MethodParameter(parameter.Name!, parameter.ParameterType));
                }
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
            Windows.Touch();
        }
        protected override void OnClosed(EventArgs e)
        {
            Windows.Touch();
            base.OnClosed(e);
        }
    }
}
