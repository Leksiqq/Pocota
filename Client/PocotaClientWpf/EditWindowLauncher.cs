using System.Windows;

namespace Net.Leksi.Pocota.Client;

public class EditWindowLauncher
{
    private readonly Dictionary<object, WeakReference<IEditWindow>> _editWindows = [];
    private readonly string _path;
    private readonly Window _owner;
    public EditWindowLauncher(string path, Window owner)
    {
        _owner = owner;
        _path = path;
        owner.Closed += Owner_Closed;
    }

    private void Owner_Closed(object? sender, EventArgs e)
    {
        foreach(WeakReference<IEditWindow> wr in _editWindows.Values)
        {
            if (
                wr.TryGetTarget(out IEditWindow? editWindow)
                && Application.Current.Windows.OfType<IEditWindow>().Any(w => w == editWindow)
            )
            {
                ((Window)editWindow).Close();
            }
        }
    }

    public IEditWindow? Launch(Property property, string? altName = null)
    {
        IEditWindow? result = null;
        if (property?.Value is { })
        {
            if (_editWindows.TryGetValue(property.Value, out WeakReference<IEditWindow>? wr))
            {
                if (
                    wr.TryGetTarget(out IEditWindow? editWindow)
                    && Application.Current.Windows.OfType<IEditWindow>().Any(w => w == editWindow)
                )
                {
                    return editWindow;
                }
                _editWindows.Remove(property.Value);
            }
            string name = !string.IsNullOrEmpty(altName) ? altName : property.Name;
            if (property is ListProperty)
            {
                result = new EditList($"{_path}/{name}", property.Type);
            }
            else
            {
                result = new EditObject($"{_path}/{name}", property.Type);
            }
            result.LaunchedBy = _owner;
            _editWindows.Add(property.Value, new WeakReference<IEditWindow>(result));
        }
        return result;
    }
    public bool IsLaunched(Property property)
    {
        if (property?.Value is { } && _editWindows.TryGetValue(property.Value!, out WeakReference<IEditWindow>? wr))
        {
            if (
                wr.TryGetTarget(out IEditWindow? editWindow)
                && Application.Current.Windows.OfType<IEditWindow>().Any(w => w == editWindow)
            )
            {
                return true;
            }
            _editWindows.Remove(property.Value);
        }
        return false;
    }
}
