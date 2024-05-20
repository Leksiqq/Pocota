﻿using System.Windows;

namespace Net.Leksi.Pocota.Client;

public class EditWindowLauncher
{
    private readonly Dictionary<string, WeakReference<IEditWindow>> _editWindows = [];
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

    public IEditWindow Launch(Property property)
    {
        if (_editWindows.TryGetValue(property.Name, out WeakReference<IEditWindow>? wr))
        {
            if (
                wr.TryGetTarget(out IEditWindow? editWindow)
                && Application.Current.Windows.OfType<IEditWindow>().Any(w => w == editWindow)
            )
            {
                return editWindow;
            }
            _editWindows.Remove(property.Name);
        }
        IEditWindow result;
        if (property is ListProperty)
        {
            result = new EditList($"{_path}/{property.Name}", property.Type);
        }
        else
        {
            result = new EditObject($"{_path}/{property.Name}", property.Type);
        }
        result.LaunchedBy = _owner;
        _editWindows.Add(property.Name, new WeakReference<IEditWindow>(result));
        return result;
    }
    public bool IsLaunched(Property property)
    {
        if (_editWindows.TryGetValue(property.Name, out WeakReference<IEditWindow>? wr))
        {
            if (
                wr.TryGetTarget(out IEditWindow? editWindow)
                && Application.Current.Windows.OfType<IEditWindow>().Any(w => w == editWindow)
            )
            {
                return true;
            }
            _editWindows.Remove(property.Name);
        }
        return false;
    }
}
