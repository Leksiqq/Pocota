using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.Resources["ServiceProvider"] as IServiceProvider)!.GetRequiredService<Window1>().Show();
        }
    }
}