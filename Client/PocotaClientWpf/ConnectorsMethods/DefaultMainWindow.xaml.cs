﻿using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.WpfMarkup;
using System.Windows;

namespace Net.Leksi.Pocota.Client;
public partial class DefaultMainWindow : Window
{
    private readonly IServiceProvider _services;
    public DataGridManager ConnectorsDataGridManager { get; private init; } = new();
    public RunMethodCommand RunCommand { get; private init; }
    public WindowsList Windows { get; private init; }
    public DefaultMainWindow()
    {
        Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        _services = (IServiceProvider)Application.Current.Resources["ServiceProvider"];
        RunCommand = new RunMethodCommand();
        ConnectorsDataGridManager.ViewSource.Source = new ConnectorsMethodsList();
        Windows = _services.GetRequiredService<WindowsList>();
        InitializeComponent();
        Windows.Touch();
    }
    protected override void OnClosed(EventArgs e)
    {
        Windows.Touch();
        base.OnClosed(e);
    }
}