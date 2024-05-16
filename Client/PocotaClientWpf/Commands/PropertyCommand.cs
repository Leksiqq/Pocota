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
            && args.Launcher is IWindowLauncher launcher
            && launcher.Launcher.IsLaunched(args.Property.Name) is bool isLaunched
            && (
                (args.Action is PropertyAction.Clear && args.Property.Value is { } && !isLaunched)
                || (args.Action is PropertyAction.Edit && args.Property.Value is { })
                || args.Action is PropertyAction.Create && args.Property.Value is null
                || args.Action is PropertyAction.Find && args.Property.Value is null
            );
    }
    public void Execute(object? parameter)
    {
        if(parameter is PropertyCommandArgs args && args.Property is { } && args.Launcher is IWindowLauncher launcher)
        {
            bool isLaunched = launcher.Launcher.IsLaunched(args.Property.Name);
            if (args.Action is PropertyAction.Create)
            {
                if (args.Property.Value is null)
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
            }
            switch (args.Action)
            {
                case PropertyAction.Clear:
                    if (!isLaunched)
                    {
                        args.Property.Value = null;
                    }
                    break;
                case PropertyAction.Edit or PropertyAction.Create:
                    if(launcher.Launcher.Launch(args.Property.Name, args.Property) is Window editWindow)
                    {
                        ((IEditWindow)editWindow).Value = args.Property.Value!;
                        editWindow.Show();
                        editWindow.Focus();
                    }
                    break;
            }
        }
    }
}
