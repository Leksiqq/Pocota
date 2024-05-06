using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Windows;
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
        public MethodWindow(MethodInfo method)
        {
            _method = method;
            _services = (IServiceProvider)Application.Current.Resources["ServiceProvider"];
            Windows = _services.GetRequiredService<WindowsList>();
            InitializeComponent();
            InitializeMethodMetrics();
            Title = $"Метод: {_method.Name}";
            Windows.Touch();
        }
        protected override void OnClosed(EventArgs e)
        {
            Windows.Touch();
            base.OnClosed(e);
        }
        private void InitializeMethodMetrics()
        {
            Paragraph par = new();
            Span returnType = new();
            returnType.Inlines.Add("Hello, world!");
            par.Inlines.Add(returnType);
            FlowDocument doc = new();
            doc.Blocks.Add(par);
            Metrics.Document = doc;
        }

    }
}
