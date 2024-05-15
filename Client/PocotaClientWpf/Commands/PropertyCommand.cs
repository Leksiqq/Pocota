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
        return parameter is PropertyCommandArgs args && args.Property is { }
            && args.Launcher is { }
            && (
                (args.Action is PropertyAction.Clear && args.Property.Value is { })
                || (args.Action is PropertyAction.Edit && args.Property.Value is { })
                || args.Action is PropertyAction.Create
                || args.Action is PropertyAction.Find
            );
    }
    public void Execute(object? parameter)
    {
        if(parameter is PropertyCommandArgs args && args.Property is { } && args.Launcher is { })
        {
            EditObject editForm;
            if(args.Action is PropertyAction.Create)
            {
                if (PocotaContext.IsEntityType(args.Property.Type))
                {
                    args.Property.Value = _services.GetRequiredService<PocotaContext>().CreateEntity(args.Property.Type);
                }
                else
                {
                    args.Property.Value = Activator.CreateInstance(args.Property.Type);
                }
            }
            switch (args.Action)
            {
                case PropertyAction.Clear:
                    args.Property.Value = null;
                    break;
                case PropertyAction.Edit or PropertyAction.Create:
                    editForm = args.Launcher.Launch(args.Property.Name);
                    editForm.Value = args.Property.Value!;
                    editForm.Show();
                    editForm.Focus();
                    break;
            }
        }
    }
}
