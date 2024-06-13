﻿using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using static Net.Leksi.Pocota.Client.Constants;
namespace Net.Leksi.Pocota.Client;
public class WindowCore : IValueConverter
{
    private readonly Window _owner;
    private readonly Localizer _localizer;
    public ApplicationCore ApplicationCore => Services.GetRequiredService<ApplicationCore>();
    public IServiceProvider Services => (IServiceProvider)Application.Current.Resources[ServiceProviderResourceKey];
    internal WindowCore(Window owner)
    {
        _owner = owner;
        _owner.Closed += _owner_Closed;
        _owner.Activated += _owner_Activated;
        _localizer = Services.GetRequiredService<Localizer>();
        Services.GetRequiredService<ApplicationCore>().Touch();
    }

    private void _owner_Activated(object? sender, EventArgs e)
    {
        ApplicationCore.ActiveWindow = _owner;
    }

    private void _owner_Closed(object? sender, EventArgs e)
    {
        Services.GetRequiredService<ApplicationCore>().Touch();
    }

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ("ThisWindow".Equals(parameter))
        {
            if (value is Tuple<int, Window> tup)
            {
                return tup.Item2 == _owner;
            }
            return false;
        }
        if ("PriorInfo".Equals(parameter))
        {
            if (value is Tuple<int, Window> tup)
            {
                return string.Format("{0,2} ", tup.Item1);
            }
            return string.Empty;
        }
        if ("Title".Equals(parameter))
        {
            if (value is Tuple<int, Window> tup)
            {
                return tup.Item2.Title;
            }
            return value?.ToString() ?? string.Empty;
        }
        if ("AdditionalInfo".Equals(parameter))
        {
            if (value is Tuple<int, Window> tup)
            {
                return tup.Item2 == _owner.Owner ? $" - {_localizer.Owner}" : ((tup.Item2 as Window)?.Owner == _owner ? $" - {_localizer.Owned}" : string.Empty);
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
