using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Windows;

namespace Net.Leksi.Pocota.Client
{
    /// <summary>
    /// Логика взаимодействия для MethodWindow.xaml
    /// </summary>
    public partial class MethodWindow : Window
    {
        private readonly MethodInfo _method;
        private readonly IServiceProvider _services;
        public string MethodMetrics { get; private init; }
        public WindowsList Windows { get; private init; }
        public MethodWindow(MethodInfo method)
        {
            _method = method;
            _services = (IServiceProvider)Application.Current.Resources["ServiceProvider"];
            MethodMetrics = InitializeMethodMetrics();
            Windows = _services.GetRequiredService<WindowsList>();
            InitializeComponent();
            Windows.Touch();
        }
        protected override void OnClosed(EventArgs e)
        {
            Windows.Touch();
            base.OnClosed(e);
        }
        private string InitializeMethodMetrics()
        {
            return _method.ToString();
        }

    }
}
