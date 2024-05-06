using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILogger<MainWindow>? _logger;
        public MainWindow()
        {
            _logger = ((IServiceProvider)Application.Current.Resources["ServiceProvider"]).GetService<ILogger<MainWindow>>();
            InitializeComponent();
            _logger?.LogInformation("launched");
        }
    }
}