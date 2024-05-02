using System.Configuration;
using System.Data;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private init; }
        public App() { }
        public App(IServiceProvider services)
        {
            ServiceProvider = services;
            InitializeComponent();
        }
    }

}
