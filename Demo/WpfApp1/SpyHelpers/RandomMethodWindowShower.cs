using Nel.Leksi.Util;
using Net.Leksi.Pocota.Client;
using Net.Leksi.Util;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1;

internal class RandomMethodWindowShower: IWpfSpyRunner
{
    private readonly List<Button> buttonList = [];
    private readonly Random _random = new();
    private readonly Queue<Tuple<WeakReference<Window>, DateTime>> _expositionQueue = [];
    private long _ticks = 0;
    private DateTime _prevShow = DateTime.MinValue;
    internal static RandomMethodWindowShower Instance { get; private set; } = new();
    internal long TimeOfWindowExpositionMs { get; set; } = 3000;
    internal long PeriodOfShowMs { get; set; } = 1000;
    private RandomMethodWindowShower() { }
    public void OnTick()
    {
        if (buttonList.Count > 0)
        {
            DateTime now = DateTime.Now;
            if ((now - _prevShow).TotalMilliseconds >= PeriodOfShowMs)
            {
                Utilities.PushButton(buttonList[_random.Next(buttonList.Count - 1)]);
                _prevShow = now;
            }
        }
        while (_expositionQueue.TryPeek(out var tup))
        {
            if (tup!.Item1.TryGetTarget(out Window? win))
            {
                if ((_prevShow - tup.Item2).TotalMilliseconds >= TimeOfWindowExpositionMs)
                {
                    _expositionQueue.Dequeue();
                    win.Close();
                }
                else
                {
                    break;
                }
            }
            else
            {
                _expositionQueue.Dequeue();
            }
        }
    }
    public void OnSpyEvent(Window window, WpfSpyEventArgs e)
    {
        if (e.EventKind == EventKind.WindowActivated && buttonList.Count == 0)
        {
            foreach (
                var item
                in
                Utilities.GetVisualDescendants(window)
                .OfType<Button>()
                .Where(b => Utilities.FindResourceKey(window.Resources, b.Style) == "PlayButtonStyle")
            )
            {
                buttonList.Add(item);
            }
        }
        else if (window is MethodWindow && e.EventKind == EventKind.WindowActivated)
        {
            //_expositionQueue.Enqueue(new Tuple<WeakReference<Window>, DateTime>(new(methodWindow), _prevShow));
        }
    }
}
