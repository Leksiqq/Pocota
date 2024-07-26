using Net.Leksi.Util;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace WpfApp2;
internal class SpyRunner : IWpfSpyRunner
{
    public void OnSpyEvent(Window window, WpfSpyEventArgs args)
    {
        Console.WriteLine($"{window}, {args.EventKind}");
        //if (args.EventKind == EventKind.WindowActivated)
        //{
        //    Utilities.GetVisualDescendants(window).OfType<ButtonBase>().First().RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        //}
    }

    public void OnTick()
    {
    }
}
