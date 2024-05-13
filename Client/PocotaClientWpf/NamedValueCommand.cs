using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;
using static Net.Leksi.Pocota.Client.Constants;

namespace Net.Leksi.Pocota.Client;

public class NamedValueCommand : ICommand
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
    public NamedValueCommand()
    {
        _services = (Application.Current.Resources[ServiceProvider] as IServiceProvider)!;
    }
    public bool CanExecute(object? parameter)
    {
        return (parameter is NamedValueCommandArgs args && args.NamedValue is { }) 
            && (
                (args.Action is NamedValueAction.Clear && args.NamedValue.Value is { })
                || (args.Action is NamedValueAction.Edit && args.NamedValue.Value is { })
                || args.Action is NamedValueAction.Create
                || args.Action is NamedValueAction.Find
            );
    }
    public void Execute(object? parameter)
    {
        if(parameter is NamedValueCommandArgs args && args.NamedValue is { })
        {
            EditObject editForm;
            switch (args.Action)
            {
                case NamedValueAction.Clear:
                    args.NamedValue.Value = null;
                    break;
                case NamedValueAction.Edit:
                    editForm = new(args.NamedValue.Value!);
                    editForm.ShowDialog();
                    break;
                case NamedValueAction.Create:
                        if (PocotaContext.IsEntityType(args.NamedValue.Type))
                        {
                            args.NamedValue.Value = _services.GetRequiredService<PocotaContext>().CreateEntity(args.NamedValue.Type);
                        }
                        else
                        {
                            args.NamedValue.Value = Activator.CreateInstance(args.NamedValue.Type);
                        }
                    editForm = new(args.NamedValue.Value!);
                    editForm.ShowDialog();
                    break;
            }
        }
    }
}
