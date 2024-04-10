using Net.Leksi.E6dWebApp;
using Net.Leksi.Pocota.Contract;
using Net.Leksi.Pocota.Tool;
using Net.Leksi.Pocota.Tool.Pages;
using System.Reflection;
using static Net.Leksi.Pocota.Tool.Constants;

public class SourceGenerator : Runner, ICommand
{
    private readonly IServiceProvider _services;
    private readonly ILogger<SourceGenerator>? _logger;
    private readonly IConfiguration _configuration;
    private string _clientLanguage = s_cSharp;
    private string _fileExtension = s_cs;
    public SourceGenerator(IServiceProvider services) : base()
    {
        _services = services;
        _logger = _services.GetService<ILogger<SourceGenerator>>();
        _configuration = _services.GetRequiredService<IConfiguration>();
    }
    public async Task Execute()
    {
        if(
            _configuration.AsEnumerable()
                .Any(
                    e => 
                        e.Key.StartsWith("argsList") 
                        && (e.Value == "-h" || e.Value == "--help")
                )
        ) 
        {
            Usage();
            return;
        }
        if (_configuration["contract-assembly"] is not string contractAssemblyPath || string.IsNullOrEmpty(contractAssemblyPath))
        {
            _logger?.LogError("Mandatory parameter missed: {par}", "--contract-assembly");
            Usage();
            return;
        }
        if(Assembly.LoadFile(contractAssemblyPath) is not Assembly contractAssembly)
        {
            _logger?.LogError("Contract assembly not found: {par}", contractAssemblyPath);
            return;
        }
        if (_configuration["client-language"] is string cl && !string.IsNullOrEmpty(cl))
        {
            if(cl != _clientLanguage)
            {
                if(cl != s_cSharp)
                {
                    _logger?.LogError("Client language is not supported: {par}", cl);
                    return;
                }
                _clientLanguage = cl;
                switch (_clientLanguage)
                {
                    case s_cSharp:
                        _fileExtension = s_cs;
                        break;
                }
            }
        }
        Start();
        IConnector connector = GetConnector();
        foreach (Type type in contractAssembly.GetTypes())
        {
            if(type.GetCustomAttribute<PocotaContractAttribute>() is PocotaContractAttribute pca)
            {
                _logger?.LogInformation("Processing contract type: {type}...", type);
                if (!type.Name.StartsWith('I'))
                {
                    _logger?.LogError("Contract name must start with 'I'.");
                    return;
                }
                await ProcessContractAsync(connector, type, pca.ContractName ?? BuildResutTypeName(type));
                _logger?.LogInformation("Done processing contract type.");
            }
        }
    }
    internal void GenerateServerModel(ServerModelModel model)
    {
        Tuple<Type, Type> types = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as Tuple<Type, Type>)!;
        string typeName = BuildResutTypeName(types.Item2);
        model.Contract = Util.BuildTypeFullName(types.Item1);
        model.ClassName = Util.GetName(typeName);
        model.NamespaceValue = Util.GetNamespace(typeName);
        model.AddInheritance(types.Item2);
        foreach(Attribute attribute in types.Item2.GetCustomAttributes())
        {
            if (!attribute.GetType().Name.StartsWith("Nullable"))
            {
                model.AddUsing(attribute.GetType());
                model.Attributes.Add(Util.BuildAttribute(attribute));
            }
        }
        NullabilityInfoContext nullabilityInfoContext = new();
        foreach (PropertyInfo pi in types.Item2.GetProperties())
        {
            PropertyModel pm = new()
            {
                Name = pi.Name,
                TypeName = Util.BuildTypeName(pi.PropertyType),
                IsNullable = nullabilityInfoContext.Create(pi).ReadState is NullabilityState.Nullable,
            };
            model.AddUsing(pi.PropertyType);
            foreach (Attribute attribute in pi.GetCustomAttributes())
            {
                if (!attribute.GetType().Name.StartsWith("Nullable"))
                { 
                    model.AddUsing(attribute.GetType());
                    pm.Attributes.Add(Util.BuildAttribute(attribute));
                }
            }
            model.Properties.Add(pm);
        }
    }
    protected override void ConfigureBuilder(WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();
        builder.Services.AddSingleton(this);
    }
    protected override void ConfigureApplication(WebApplication app)
    {
        app.MapRazorPages();
    }
    private async Task ProcessContractAsync(IConnector connector, Type type, string name)
    {
        if (_configuration["controllers"] is string controllersFolder && !string.IsNullOrEmpty(controllersFolder))
        {
            if (!Directory.Exists(controllersFolder))
            {
                Directory.CreateDirectory(controllersFolder);
            }
            await GenerateControllerAsync(connector, type, controllersFolder, name);
        }
        if (_configuration["services"] is string servicesFolder && !string.IsNullOrEmpty(servicesFolder))
        {
            if (!Directory.Exists(servicesFolder))
            {
                Directory.CreateDirectory(servicesFolder);
            }
            await GenerateServiceInterfaceAsync(connector, type, servicesFolder, name);
        }
        if (_configuration["connectors"] is string connectorsFolder && !string.IsNullOrEmpty(connectorsFolder))
        {
            if (!Directory.Exists(connectorsFolder))
            {
                Directory.CreateDirectory(connectorsFolder);
            }
            await GenerateConnectorAsync(connector, type, connectorsFolder, name);
        }
        foreach (EntityAttribute entityAttribute in type.GetCustomAttributes<EntityAttribute>())
        {
            if (_configuration["server-models"] is string serverModelsFolder && !string.IsNullOrEmpty(serverModelsFolder))
            {
                if (!Directory.Exists(serverModelsFolder))
                {
                    Directory.CreateDirectory(serverModelsFolder);
                }
                await GenerateServerModelAsync(connector, type, entityAttribute.EntityType, serverModelsFolder);
            }
            if (_configuration["client-models"] is string clientModelsFolder && !string.IsNullOrEmpty(clientModelsFolder))
            {
                if (!Directory.Exists(clientModelsFolder))
                {
                    Directory.CreateDirectory(clientModelsFolder);
                }
                await GenerateClientModelAsync(connector, type, entityAttribute.EntityType, clientModelsFolder);
            }
        }
    }

    private async Task GenerateConnectorAsync(IConnector connector, Type type, string targetFolder, string name)
    {
        switch (_clientLanguage)
        {
            case s_cSharp:
                await GenerateCSharpConnectorAsync(connector, type, targetFolder, name);
                break;
        }
    }
    private async Task GenerateCSharpConnectorAsync(IConnector connector, Type type, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating {lang} connector {name}Connector at {folder}.", _clientLanguage, name, targetFolder);
        _logger?.LogInformation("Done.");
        await Task.CompletedTask;
    }

    private async Task GenerateClientModelAsync(IConnector connector, Type contractType, Type entityType, string targetFolder)
    {
        switch (_clientLanguage)
        {
            case s_cSharp:
                await GenerateCSharpClientModelAsync(connector, contractType, entityType, targetFolder);
                break;
        }
    }
    private async Task GenerateCSharpClientModelAsync(IConnector connector, Type contractType, Type entityType, string targetFolder)
    {
        string name = BuildResutTypeName(entityType);
        _logger?.LogInformation("Generating {lang} client's entity model {name} at {folder}.", _clientLanguage, name, targetFolder);
        _logger?.LogInformation("Done.");
        await Task.CompletedTask;
    }
    private async Task GenerateServerModelAsync(IConnector connector, Type contractType, Type entityType, string targetFolder)
    {
        string name = BuildResutTypeName(entityType);
        _logger?.LogInformation("Generating server's entity model {name} at {folder}.", name, targetFolder);
        await ProcessPageAsync(connector, contractType, entityType, "/ServerModel", BuildFilePath(targetFolder, name));
        _logger?.LogInformation("Done.");
        await Task.CompletedTask;
    }
    private string BuildFilePath(string targetFolder, string name)
    {
        string typeName = Util.GetName(name);
        return $"{Path.Combine(targetFolder, typeName)}{_fileExtension}";
    }
    private async Task ProcessPageAsync(IConnector connector, Type contractType, Type entityType, string uri, string path)
    {
        HttpRequestMessage request = new(HttpMethod.Get, uri);
        HttpResponseMessage response = connector.Send(request, new Tuple<Type, Type>(contractType, entityType));
        Stream input = await response.Content.ReadAsStreamAsync();
        using FileStream output = new(path, FileMode.Create);
        await input.CopyToAsync(output);
    }
    private async Task GenerateServiceInterfaceAsync(IConnector connector, Type type, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating service interface I{name}Service at {folder}.", name, targetFolder);
        _logger?.LogInformation("Done.");
        await Task.CompletedTask;
    }

    private async Task GenerateControllerAsync(IConnector connector, Type contract, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating controller {name}Controller at {folder}.", name, targetFolder);
        _logger?.LogInformation("Done.");
        await Task.CompletedTask;
    }
    private static string BuildResutTypeName(Type entityType)
    {
        return $"{(string.IsNullOrEmpty(entityType.Namespace) ? string.Empty : $"{entityType.Namespace}.")}{entityType.Name[1..]}";
    }
    private void Usage()
    {

    }
 }