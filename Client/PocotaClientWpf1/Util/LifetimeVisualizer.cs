using Net.Leksi.Util;
using System.Collections.Concurrent;
using System.Windows;

namespace Net.Leksi.Pocota.Client;
public static class LifetimeVisualizer
{
    private static Dictionary<Type, CountHolder> s_counts = [];
    private static int step = 0;
    private static bool _running = false;
    public static void Start()
    {
        if (!_running)
        {
            _running = true;
            LifetimeObserver lifetimeObserver = Application.Current.GetRequiredService<LifetimeObserver>();
            lock (s_counts)
            {
                foreach (Type type in lifetimeObserver.GetTracedTypes())
                {
                    s_counts.Add(type, new CountHolder());
                }
            }
            lifetimeObserver.LifetimeEventOccured += (s, e) =>
            {
                lock (e.Type)
                {
                    CountHolder ch = s_counts[e.Type];
                    switch (e.Kind)
                    {
                        case LifetimeEventKind.Created:
                            Interlocked.Increment(ref ch._incCount);
                            break;
                        case LifetimeEventKind.Finalized:
                            Interlocked.Increment(ref ch._decCount);
                            break;
                    };
                }
                lock (s_counts)
                {
                    Console.SetCursorPosition(0, 0);
                    foreach (var item in s_counts)
                    {
                        string line = $"{item.Key}:\t+{item.Value._incCount}, -{item.Value._decCount}, {item.Value._incCount - item.Value._decCount}";
                        string blank = string.Format($"{{0,{line.Length + 5}}}\r", string.Empty);
                        Console.WriteLine($"{blank}{line}");
                    }
                }
                if (Interlocked.Increment(ref step) % 100 == 0)
                {
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive);
                    GC.WaitForPendingFinalizers();
                }
            };
        }
    }
}

internal class CountHolder
{
    internal int _incCount = 0;
    internal int _decCount = 0;
}