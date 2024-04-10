using Net.Leksi.Pocota.Tool;

internal class CommandExecutor : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<CommandExecutor>? _logger;
    public CommandExecutor(IServiceProvider services)
    {
        _services = services;
        _logger = _services.GetService<ILogger<CommandExecutor>>();
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _services.GetRequiredService<ICommand>().Execute();
        }
        catch(Exception ex)
        {
            _logger?.LogCritical(ex, string.Empty);
        }
        finally
        {
            await _services.GetRequiredService<IHost>().StopAsync(stoppingToken);
        }
    }
}