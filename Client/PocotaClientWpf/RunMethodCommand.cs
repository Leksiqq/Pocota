using System.Reflection;
using System.Windows.Input;

namespace Net.Leksi.Pocota.Client;

public class RunMethodCommand : ICommand
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
        return parameter is MethodInfo;
    }

    public void Execute(object? parameter)
    {
        if(parameter is MethodInfo mi)
        {
            MethodWindow methodWindow = new(mi);
            methodWindow.Show();
        }
    }
}
