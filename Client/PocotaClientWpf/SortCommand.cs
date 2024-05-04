using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Net.Leksi.Pocota.Client;

public class SortCommand(CollectionViewSource source) : ICommand, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
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
    public int Notification 
    {  
        get
        {
            return 0;
        }
    }


    public bool CanExecute(object? parameter)
    {
        return true;
    }
    public void Execute(object? parameter)
    {
        if (
            parameter is object[] parameters
            && parameters.Length > 1
            && parameters[0] is Button button
            && parameters[1] is string field
        )
        {
            if (button.Tag is bool b)
            {
                if (!b)
                {
                    button.Tag = true;
                }
                else
                {
                    button.Tag = null;
                }
            }
            else {
                button.Tag = false;
            }
            if (button.Tag is bool b1)
            {
                if(
                    Enumerable.Range(0, source.SortDescriptions.Count).Where(i => source.SortDescriptions[i].PropertyName == field)
                        .FirstOrDefault(-1) is int pos && pos >= 0
                )
                {
                    source.SortDescriptions[pos] = new SortDescription
                    {
                        PropertyName = field,
                        Direction = b1 ? ListSortDirection.Descending : ListSortDirection.Ascending,
                    };
                }
                else
                {
                    source.SortDescriptions.Add(new SortDescription
                    {
                        PropertyName = field,
                        Direction = b1 ? ListSortDirection.Descending : ListSortDirection.Ascending,
                    });
                }
            } 
            else if(source.SortDescriptions.Where(d => d.PropertyName == field).FirstOrDefault() is SortDescription sd)
            {
                source.SortDescriptions.Remove(sd);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Notification)));
        }
    }
}
