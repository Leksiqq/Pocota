using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Net.Leksi.Pocota.Client;

public class WindowsList: ObservableCollection<Window>, ICommand
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
    private readonly IServiceProvider _services;
    private readonly Application _application;
    private readonly HashSet<Window> _windowsToRemove = [];
    public WindowsList(IServiceProvider services)
    {
        _services = services;
        _application = _services.GetRequiredService<Application>();
    }
    public bool CanExecute(object? parameter)
    {
        return parameter is Window;
    }

    public void Execute(object? parameter)
    {
        if(parameter is Window window)
        {
            window.Activate();
        }
    }
    public void Touch()
    {
        _windowsToRemove.Clear();
        foreach(var item in this)
        {
            _windowsToRemove.Add(item);
        }
        foreach (var item in _application.Windows)
        {
            Window window = (Window)item;
            if(!_windowsToRemove.Remove(window))
            {
                Add(window);
            }
        }
        foreach(var item in _windowsToRemove)
        {
            Remove(item);
        }
    }

}
