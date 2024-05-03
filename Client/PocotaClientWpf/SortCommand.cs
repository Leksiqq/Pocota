using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Net.Leksi.Pocota.Client;

public class SortCommand(CollectionViewSource source) : ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
        }
    }
    private readonly Dictionary<string,ToggleButton> _buttons = [];

    public bool CanExecute(object? parameter)
    {
        if (
            parameter is object[] parameters
            && parameters.Length > 1
            && parameters[0] is ToggleButton button
            && parameters[1] is string field
        )
        {
            _buttons.TryAdd(field, button);
            return true;
        }
        return false;
    }

    public void Execute(object? parameter)
    {
        if(
            parameter is object[] parameters 
            && parameters.Length > 1 
            && parameters[0] is ToggleButton button 
            && parameters[1] is string field
        )
        {
            Console.WriteLine(button.ToolTip);
            _buttons.TryAdd(field, button);
            KeyStates keyStates = Keyboard.GetKeyStates(Key.LeftCtrl) | Keyboard.GetKeyStates(Key.RightCtrl);
            if(!keyStates.HasFlag(KeyStates.Down))
            {
                bool? isChecked = button.IsChecked;
                //ResetButtonsOrder();
                button.IsChecked = isChecked;
                source.SortDescriptions.Clear();
                if (button.IsChecked is bool b)
                {
                    source.SortDescriptions.Add(new SortDescription
                    {
                        PropertyName = field,
                        Direction = b ? ListSortDirection.Descending : ListSortDirection.Ascending,
                    });
                }
            }
            else
            {
                source.SortDescriptions.Add(new SortDescription
                {
                    PropertyName = "field",
                    Direction = ListSortDirection.Descending,
                });
            }
        }
    }
    private void ResetButtonsOrder()
    {
        DependencyObject? dg = null;
        foreach (ToggleButton button in _buttons.Values)
        {
            if (dg is null)
            {
                for (dg = button; dg != null && dg is not DataGrid; dg = VisualTreeHelper.GetParent(dg)) { }
            }
            DependencyObject cur = button;
            for (; cur != null && cur is not StackPanel; cur = VisualTreeHelper.GetParent(cur)) { }
            int cnt = VisualTreeHelper.GetChildrenCount(cur);
            for (int i = 0; i < cnt; ++i)
            {
                if (VisualTreeHelper.GetChild(cur, i) is StackPanel sp && sp.Name == "SortOrder")
                {
                    var descriptor = DependencyPropertyDescriptor.FromName(
                        "Visibility",
                        sp.GetType(),
                        sp.GetType()
                    );

                    descriptor.SetValue(sp, Visibility.Hidden);
                    break;
                }
            }
            //button.IsChecked = null;

            Console.WriteLine(button.ToolTip);
        }
        (dg as DataGrid)?.InvalidateVisual();
    }
}
