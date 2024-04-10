using Net.Leksi.Pocota.Tool;

List<KeyValuePair<string, string>> argsList = [];
for(int i = 0; i < args.Length; ++i)
{
    argsList.Add(new KeyValuePair<string, string>($"argsList{i}", args[i]));
}
    

IConfiguration bootstrapConfig = new ConfigurationBuilder()
    .AddCommandLine(args)
    .AddInMemoryCollection(argsList!)
    .Build();

if(args.Contains("-h") || args.Contains("--help"))
{
    Usage();
    return;
}

if (args.Length == 0 || args[0].StartsWith("--") || args[0].StartsWith('/'))
{
    Usage();
    return;
}

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(bootstrapConfig);
builder.Services.AddHostedService<CommandExecutor>();

if (args[0] == "gen")
{
    builder.Services.AddSingleton<ICommand, SourceGenerator>();
}
else
{
    Usage();
    return;
}

IHost host = builder.Build();
await host.StartAsync();

void Usage()
{
    Console.WriteLine(@$"
Usage: pocota [command] [options]

Commands: 
  gen   Generate source files from contract.

Options:
  -h|--help  Show help information.

Use ""pocota [command] --help"" for more information about a command.
");
}