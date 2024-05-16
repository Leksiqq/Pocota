using System.Windows;

namespace Net.Leksi.Pocota.Client;

public class EditWindowLauncher(string path, Window owner)
{
    private readonly Dictionary<string, WeakReference<IEditWindow>> _editWindows = [];
    public IEditWindow Launch(string parameterName, Property property)
    {
        if (_editWindows.TryGetValue(parameterName, out WeakReference<IEditWindow>? wr))
        {
            if (
                wr.TryGetTarget(out IEditWindow? editWindow)
                && Application.Current.Windows.OfType<IEditWindow>().Any(w => w == editWindow)
            )
            {
                return editWindow;
            }
            _editWindows.Remove(parameterName);
        }
        IEditWindow result;
        if (property is ListProperty)
        {
            result = new EditList($"{path}/{parameterName}", property.Type);
        }
        else
        {
            result = new EditObject($"{path}/{parameterName}", property.Type);
        }
        result.LaunchedBy = owner;
        _editWindows.Add(parameterName, new WeakReference<IEditWindow>(result));
        return result;
    }
    public bool IsLaunched(string parameterName)
    {
        if (_editWindows.TryGetValue(parameterName, out WeakReference<IEditWindow>? wr))
        {
            if (
                wr.TryGetTarget(out IEditWindow? editWindow)
                && Application.Current.Windows.OfType<IEditWindow>().Any(w => w == editWindow)
            )
            {
                return true;
            }
            _editWindows.Remove(parameterName);
        }
        return false;
    }
}
