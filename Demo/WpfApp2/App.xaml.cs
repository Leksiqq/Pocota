using Nel.Leksi.Util;
using Net.Leksi.Util;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            WpfSpy.Instance.WpfEventOccured += Instance_WpfEventOccured;
            WpfSpy.Instance.Start();
            base.OnStartup(e);
        }

        private void Instance_WpfEventOccured(object? sender, Net.Leksi.Util.WpfSpyEventArgs args)
        {
            Console.WriteLine($"{sender}, {args.EventKind}");
            if (args.EventKind == Net.Leksi.Util.EventKind.WindowActivated) 
            {
                Utilities.GetVisualDescendants(sender as Window).OfType<ButtonBase>().First().RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }
    }

}
