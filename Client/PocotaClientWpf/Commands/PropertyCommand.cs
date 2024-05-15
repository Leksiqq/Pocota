using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;
using static Net.Leksi.Pocota.Client.Constants;

namespace Net.Leksi.Pocota.Client;

public class PropertyCommand : ICommand
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
    public PropertyCommand()
    {
        _services = (Application.Current.Resources[ServiceProvider] as IServiceProvider)!;
    }
    public bool CanExecute(object? parameter)
    {
        return (parameter is PropertyCommandArgs args && args.Property is { }) 
            && (
                (args.Action is PropertyAction.Clear && args.Property.Value is { })
                || (args.Action is PropertyAction.Edit && args.Property.Value is { })
                || args.Action is PropertyAction.Create
                || args.Action is PropertyAction.Find
            );
    }
    public void Execute(object? parameter)
    {
        if(parameter is PropertyCommandArgs args && args.Property is { })
        {
            EditObject editForm;
            switch (args.Action)
            {
                case PropertyAction.Clear:
                    args.Property.Value = null;
                    break;
                case PropertyAction.Edit:
                    editForm = new(args.Property.Value!);
                    if(args.Launcher is { })
                    {
                        editForm.Launcher = args.Launcher;
                    }
                    editForm.Show();
                    break;
                case PropertyAction.Create:
                        if (PocotaContext.IsEntityType(args.Property.Type))
                        {
                            args.Property.Value = _services.GetRequiredService<PocotaContext>().CreateEntity(args.Property.Type);
                        }
                        else
                        {
                            args.Property.Value = Activator.CreateInstance(args.Property.Type);
                        }
                    editForm = new(args.Property.Value!);
                    if (args.Launcher is { })
                    {
                        editForm.Launcher = args.Launcher;
                    }
                    editForm.Show();
                    break;
            }
        }
    }
}
