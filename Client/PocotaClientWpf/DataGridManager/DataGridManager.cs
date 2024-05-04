using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Net.Leksi.Pocota.Client;

public partial class DataGridManager: INotifyPropertyChanged
{
    public class SortByColumn(DataGridManager manager) : ICommand
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
            return (parameter is SortByColumnArgs);
        }

        public void Execute(object? parameter)
        {
            if (parameter is SortByColumnArgs args && args.FieldName is { } && args.Button is { })
            {
                manager.SortByColumnExecute(args);
            }
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    public CollectionViewSource ViewSource { get; private init; } = new();
    public SortByColumn SortByColumnCommand { get; private init; }
    public int Notification => 0;
    public DataGridManager()
    {
        SortByColumnCommand = new SortByColumn(this);
    }
    internal void SortByColumnExecute(SortByColumnArgs args)
    {
        if (args.Button!.Tag is bool b)
        {
            if (!b)
            {
                args.Button.Tag = true;
            }
            else
            {
                args.Button.Tag = null;
            }
        }
        else
        {
            args.Button.Tag = false;
        }
        if (args.Button.Tag is bool b1)
        {
            if (
                Enumerable.Range(0, ViewSource.SortDescriptions.Count)
                    .Where(i => ViewSource.SortDescriptions[i].PropertyName == args.FieldName)
                    .FirstOrDefault(-1) is int pos && pos >= 0
            )
            {
                ViewSource.SortDescriptions[pos] = new SortDescription
                {
                    PropertyName = args.FieldName,
                    Direction = b1 ? ListSortDirection.Descending : ListSortDirection.Ascending,
                };
            }
            else
            {
                ViewSource.SortDescriptions.Add(new SortDescription
                {
                    PropertyName = args.FieldName,
                    Direction = b1 ? ListSortDirection.Descending : ListSortDirection.Ascending,
                });
            }
        }
        else if (ViewSource.SortDescriptions.Where(d => d.PropertyName == args.FieldName).FirstOrDefault() is SortDescription sd)
        {
            ViewSource.SortDescriptions.Remove(sd);
        }
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Notification)));

    }

    internal object Convert(string? fieldName, object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter.ToString() == "sortPositionVisibility")
        {
            if (
                Enumerable.Range(0, ViewSource.SortDescriptions.Count)
                    .Where(i => ViewSource.SortDescriptions[i].PropertyName == fieldName)
                    .FirstOrDefault(-1) is int pos
                && pos >= 0
            )
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
        else if (parameter.ToString() == "sortPositionText")
        {
            if (
                Enumerable.Range(0, ViewSource.SortDescriptions.Count)
                    .Where(i => ViewSource.SortDescriptions[i].PropertyName == fieldName)
                    .FirstOrDefault(-1) is int pos
                && pos >= 0
            )
            {
                return (pos + 1).ToString();
            }
            return string.Empty;
        }
        else if (parameter.ToString() == "tag")
        {
            return (bool?)value;
        }
        else
        {
            Console.WriteLine($"Convert {GetHashCode()}: {value}, {fieldName}, {parameter}");
        }
        return value;
    }
}
