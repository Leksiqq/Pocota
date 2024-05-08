using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Net.Leksi.Pocota.Client
{
    /// <summary>
    /// Логика взаимодействия для MethodWindow.xaml
    /// </summary>
    public partial class MethodWindow : Window
    {
        private readonly MethodInfo _method;
        private readonly IServiceProvider _services;
        public WindowsList Windows { get; private init; }
        public string MethodName => _method is { } ? $"{Util.BuildTypeName(_method.DeclaringType!)}.{_method.Name}" : string.Empty;
        public MethodWindow(MethodInfo method)
        {
            _method = method;
            _services = (IServiceProvider)Application.Current.Resources["ServiceProvider"];
            Windows = _services.GetRequiredService<WindowsList>();
            InitializeComponent();
            InitializeMethodMetrics();
            Windows.Touch();
        }
        private void InitializeMethodMetrics()
        {
            Metrics.MethodInfo = _method;
        }

        protected override void OnClosed(EventArgs e)
        {
            Windows.Touch();
            base.OnClosed(e);
        }
    }
}
