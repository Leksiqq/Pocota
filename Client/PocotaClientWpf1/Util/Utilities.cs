using System.Windows.Media;
using System.Windows;

namespace Net.Leksi.Pocota.Client;
internal static class Utilities
{
    internal static IEnumerable<DependencyObject> GetVisualDescendants(DependencyObject obj)
    {
        int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
        for (int i = 0; i < childrenCount; ++i)
        {
            DependencyObject child = VisualTreeHelper.GetChild(obj, i);
            yield return child;
            foreach (var descendant in GetVisualDescendants(child))
            {
                yield return descendant;
            }
        }
    }
}
