using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Net.Leksi.Pocota.Client;

public class ApplicationCore: ICommand, INotifyPropertyChanged
{
    private readonly PropertyChangedEventArgs _propertyChangedEventsArg = new(null);

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
    public IEnumerable<Window> Windows
    {
        get
        {
            foreach (Window window in Application.Current.Windows)
            {
                yield return window;
            }
        }
    }
    public bool CanExecute(object? parameter)
    {
        return parameter is Window;
    }
    public void Execute(object? parameter)
    {
        if (parameter is Window window)
        {
            window.Activate();
        }
    }
    internal void Touch()
    {
        PropertyChanged?.Invoke(this, _propertyChangedEventsArg);
    }
}
