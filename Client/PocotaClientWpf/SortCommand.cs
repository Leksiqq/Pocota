using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

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

    public bool CanExecute(object? parameter)
    {
        return true;
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
    }
}
