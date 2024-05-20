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
        if(parameter is PropertyCommandArgs args)
        {
            if(args.Property is { })
            {
                if(args.Launcher is IWindowLauncher launcher)
                {
                    if(launcher.Launcher.IsLaunched(args.Property) is bool isLaunched)
                    {
                        if (
                            (args.Action is PropertyAction.Clear && args.Property.Value is { } && !isLaunched)
                            || (args.Action is PropertyAction.Edit && args.Property.Value is { })
                            || args.Action is PropertyAction.Create && args.Property.Value is null
                            || args.Action is PropertyAction.Find && args.Property.Value is null
                        )
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    public void Execute(object? parameter)
    {
        if(parameter is PropertyCommandArgs args && args.Property is { } && args.Launcher is IWindowLauncher launcher)
        {
            bool isLaunched = launcher.Launcher.IsLaunched(args.Property);
            if (args.Action is PropertyAction.Create)
            {
                if (args.Property.Value is null)
                {
                    if (PocotaContext.IsEntityType(args.Property.Type))
                    {
                        args.Property.Value = _services.GetRequiredService<PocotaContext>().CreateEntity(args.Property.Type);
                    }
                    else if(args.Property.Type.IsClass && args.Property.Type != typeof(string))
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
                    if(launcher.Launcher.Launch(args.Property) is Window editWindow)
                    {
                        ((IEditWindow)editWindow).Property = args.Property;
                        editWindow.Show();
                        editWindow.Focus();
                    }
                    break;
            }
        }
    }
}
