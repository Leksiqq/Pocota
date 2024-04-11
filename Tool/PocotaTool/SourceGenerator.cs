using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Net.Leksi.E6dWebApp;
using Net.Leksi.Pocota.Contract;
using Net.Leksi.Pocota.Tool;
using Net.Leksi.Pocota.Tool.Pages;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Net.Leksi.Pocota.Tool.Constants;

namespace Net.Leksi.Pocota;
public class SourceGenerator : Runner, ICommand
{
    private readonly IServiceProvider _services;
    private readonly ILogger<SourceGenerator>? _logger;
    private readonly IConfiguration _configuration;
    private readonly string[] _serverFolders = ["contexts", "controllers", "services", "server-models", "server-converters", "server-extensions"];
    private readonly string[] _clientFolders = ["connectors", "client-models", "client-converters", "client-extensions"];
    private readonly string _clientLanguage = s_cSharp;
    private readonly string _fileExtension = s_cs;
    public SourceGenerator(IServiceProvider services) : base()
    {
        _services = services;
        _logger = _services.GetService<ILogger<SourceGenerator>>();
        _configuration = _services.GetRequiredService<IConfiguration>();
        if (_configuration["client-language"] is string cl && !string.IsNullOrEmpty(cl))
        {
            if (cl != _clientLanguage)
            {
                if (cl != s_cSharp)
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
        if (_configuration["server-base"] is string sb && !string.IsNullOrEmpty(sb))
        {
            foreach (string folder in _serverFolders)
            {
                if (_configuration[folder] is null)
                {
                    _configuration[folder] = Path.Combine(sb, PascalCase(folder.StartsWith("server-") ? folder["server-".Length..] : folder));
                }
                else if (string.IsNullOrWhiteSpace(_configuration[folder]))
                {
                    _configuration[folder] = sb;
                }
                else
                {
                    _configuration[folder] = Path.Combine(sb, _configuration[folder]!);
                }
            }
        }
        if (_configuration["client-base"] is string cb && !string.IsNullOrEmpty(cb))
        {
            foreach (string folder in _clientFolders)
            {
                if (_configuration[folder] is null)
                {
                    _configuration[folder] = Path.Combine(cb, PascalCase(folder.StartsWith("client-") ? folder["client-".Length..] : folder));
                }
                else if (string.IsNullOrWhiteSpace(_configuration[folder]))
                {
                    _configuration[folder] = cb;
                }
                else
                {
                    _configuration[folder] = Path.Combine(cb, _configuration[folder]!);
                }
            }
        }
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
        if (string.IsNullOrEmpty(_configuration["server-base"]) && _serverFolders.Any(f => !string.IsNullOrEmpty(_configuration[f])))
        {
            _logger?.LogError("Mandatory parameter missed: {par}", "--server-base");
            Usage();
            return;
        }
        if (string.IsNullOrEmpty(_configuration["client-base"]) && _clientFolders.Any(f => !string.IsNullOrEmpty(_configuration[f])))
        {
            _logger?.LogError("Mandatory parameter missed: {par}", "--client-base");
            Usage();
            return;
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
    internal static void PopulateServerModelModel(ServerModelModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        string typeName = BuildResutTypeName(options.EntityType!);
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(typeName);
        model.NamespaceValue = Util.GetNamespace(typeName);
        HashSet<Type> entityTypes = [];
        foreach (EntityAttribute attribute in options.ContractType!.GetCustomAttributes<EntityAttribute>())
        {
            entityTypes.Add(attribute.EntityType);
        }
        HashSet<Type> usingsAtAttributes = [];
        foreach(Attribute attribute in options.EntityType!.GetCustomAttributes())
        {
            if (!attribute.GetType().Name.StartsWith("Nullable"))
            {
                model.AddUsing(attribute.GetType());
                model.Attributes.Add(Util.BuildAttribute(attribute, usingsAtAttributes));
            }
        }
        NullabilityInfoContext nullabilityInfoContext = new();
        foreach (PropertyInfo pi in options.EntityType!.GetProperties())
        {
            PropertyModel pm = new()
            {
                Name = pi.Name,
                TypeName = BuildTypeName(pi.PropertyType, model, entityTypes),
                IsNullable = nullabilityInfoContext.Create(pi).ReadState is NullabilityState.Nullable,
            };
            model.AddUsing(pi.PropertyType);
            foreach (Attribute attribute in pi.GetCustomAttributes())
            {
                if (!attribute.GetType().Name.StartsWith("Nullable"))
                { 
                    model.AddUsing(attribute.GetType());
                    pm.Attributes.Add(Util.BuildAttribute(attribute, usingsAtAttributes));
                }
            }
            model.Properties.Add(pm);
        }
        foreach(Type type in usingsAtAttributes)
        {
            model.AddUsing(type);
        }
    }
    internal static void PopulateContextModel(ContextModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = Util.GetNamespace(options.ClassName!);
        model.AddInheritance(typeof(DbContext));
        model.AddUsing(typeof(DbContextOptions<>));
        model.AddUsing(typeof(DbSet<>));
        foreach (EntityAttribute attribute in options.ContractType!.GetCustomAttributes<EntityAttribute>())
        {
            model.AddUsing(attribute.EntityType);
            PropertyModel pm = new()
            {
                Name = !string.IsNullOrEmpty(attribute.NameOfSet) ? attribute.NameOfSet 
                    : $"SetOf{Util.GetTypeName(BuildResutTypeName(attribute.EntityType))}",
                TypeName = Util.GetTypeName(BuildResutTypeName(attribute.EntityType)),
            };
            model.Properties.Add(pm);
        }
    }
    internal void PopulateServiceBaseModel(ServiceBaseModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = Util.GetNamespace(options.ClassName!);
        HashSet<Type> entityTypes = [];
        foreach (EntityAttribute attribute in options.ContractType!.GetCustomAttributes<EntityAttribute>())
        {
            entityTypes.Add(attribute.EntityType);
        }
        foreach(MethodInfo mi in options.ContractType!.GetMethods())
        {
            string returnTypeName = BuildTypeName(mi.ReturnType, model, entityTypes);
            if (mi.ReturnType.IsGenericType)
            {
                model.AddUsing(typeof(IAsyncEnumerable<>));
                returnTypeName = returnTypeName.Replace(nameof(IEnumerable<object>), nameof(IAsyncEnumerable<object>));
            }
            else
            {
                model.AddUsing(typeof(ValueTask<>));
                returnTypeName = $"{nameof(ValueTask<object>)}<{returnTypeName}>";
            }
            MethodModel mm = new()
            {
                Name = $"{mi.Name}Async",
                ReturnTypeName = returnTypeName,
            };
            foreach(ParameterInfo pi in mi.GetParameters())
            {
                ParameterModel pm = new()
                {
                    Name = pi.Name!,
                    TypeName = BuildTypeName(pi.ParameterType, model, entityTypes),
                };
                mm.Parameters.Add(pm);
            }
            model.Methods.Add(mm);
        }
    }
    internal static void PopulateControllerModel(ControllerModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = Util.GetNamespace(options.ClassName!);
        model.ServiceClassName = model.ClassName.Replace("Controller", "ServiceBase");
        model.JsonConverterFactoryClassName = model.ClassName.Replace("Controller", "JsonConverterFactory");
        model.AddInheritance(typeof(ControllerBase));
        model.AddUsing(typeof(JsonSerializer));
        model.AddUsing(typeof(JsonSerializerOptions));
        model.AddUsing(typeof(ReferenceHandler));
        model.AddUsing(typeof(ApiControllerAttribute));
        model.AddUsing(typeof(RouteAttribute));
        model.AddUsing(typeof(HttpGetAttribute));
        model.AddUsing(typeof(HttpPostAttribute));
        HashSet<Type> entityTypes = [];
        foreach (EntityAttribute attribute in options.ContractType!.GetCustomAttributes<EntityAttribute>())
        {
            entityTypes.Add(attribute.EntityType);
        }
        foreach (MethodInfo mi in options.ContractType!.GetMethods())
        {
            MethodModel mm = new()
            {
                Name = mi.Name,
                ReturnTypeName = BuildTypeName(mi.ReturnType, model, entityTypes),
                IsEnumeration = mi.ReturnType.IsGenericType,
            };
            mm.Attributes.Add($"[HttpGet(\"{mi.Name}{(mi.GetParameters().Length > 0 ? $"/{string.Join('/', mi.GetParameters().Select(p => $"{{{p.Name}}}"))}" : string.Empty)}\")]");
            foreach (ParameterInfo pi in mi.GetParameters())
            {
                ParameterModel pm = new()
                {
                    Name = pi.Name!,
                    TypeName = BuildTypeName(pi.ParameterType, model, entityTypes),
                };
                mm.Parameters.Add(pm);
            }
            model.Methods.Add(mm);
        }
        model.Attributes.Add($"[ApiController]");
        model.Attributes.Add($"[Route(\"/{model.ClassName.Replace("Controller", string.Empty)}\")]");
    }
    internal static void PopulateJsonConverterFactoryModel(JsonConverterFactoryModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = Util.GetNamespace(options.ClassName!);
        model.AddUsing(typeof(HashSet<>));
        model.AddInheritance(typeof(JsonConverterFactory));
        model.AddUsing(typeof(JsonConverter));
        model.AddUsing(typeof(JsonSerializerOptions));
        model.AddUsing(typeof(IServiceProvider));
        foreach (EntityAttribute attribute in options.ContractType!.GetCustomAttributes<EntityAttribute>())
        {
            model.AddUsing(attribute.EntityType);
            model.Entities.Add(Util.GetTypeName(BuildResutTypeName(attribute.EntityType)));
        }
    }
    internal static void PopulateJsonConverterModel(JsonConverterModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = Util.GetNamespace(options.ClassName!);
        model.AddUsing(typeof(JsonConverter<>));
        model.AddUsing(typeof(JsonSerializer));
        model.AddUsing(typeof(IServiceProvider));
        model.AddUsing(options.ContractType!);
        model.EntityTypeName = Util.GetTypeName(BuildResutTypeName(options.EntityType!));
        model.ContractName = options.ContractType!.GetCustomAttribute<PocotaContractAttribute>()!.ContractName!;
        HashSet<Type> entityTypes = [];
        foreach (EntityAttribute attribute in options.ContractType!.GetCustomAttributes<EntityAttribute>())
        {
            model.AddUsing(attribute.EntityType);
            entityTypes.Add(attribute.EntityType);
        }
        model.AddInheritance(BuildTypeName(typeof(JsonConverter<>).MakeGenericType([options.EntityType!]), model, entityTypes));
        foreach (PropertyInfo pi in options.EntityType!.GetProperties())
        {
            PropertyModel pm = new()
            {
                Name = pi.Name,
            };
            model.Properties.Add(pm);
        }
    }
    internal static void PopulateExtensionsModel(ExtensionsModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = Util.GetNamespace(options.ClassName!);
        model.ContractName = options.ContractType!.GetCustomAttribute<PocotaContractAttribute>()!.ContractName!;
        model.AddUsing(typeof(IServiceCollection));
        foreach (EntityAttribute attribute in options.ContractType!.GetCustomAttributes<EntityAttribute>())
        {
            model.AddUsing(attribute.EntityType);
            PropertyModel pm = new()
            {
                Name = Util.GetTypeName(BuildResutTypeName(attribute.EntityType)),
            };
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
    private async Task ProcessContractAsync(IConnector connector, Type contractType, string name)
    {
        if (_configuration["contexts"] is string contextsFolder && !string.IsNullOrEmpty(contextsFolder))
        {
            if (!Directory.Exists(contextsFolder))
            {
                Directory.CreateDirectory(contextsFolder);
            }
            await GenerateContextAsync(connector, contractType, contextsFolder, $"{name}Context");
        }
        if (_configuration["server-extensions"] is string serverExtensionsFolder && !string.IsNullOrEmpty(serverExtensionsFolder))
        {
            if (!Directory.Exists(serverExtensionsFolder))
            {
                Directory.CreateDirectory(serverExtensionsFolder);
            }
            await GenerateServerExtensionsAsync(connector, contractType, serverExtensionsFolder, $"{name}Extensions");
        }
        if (_configuration["server-converters"] is string serverConvertersFolder && !string.IsNullOrEmpty(serverConvertersFolder))
        {
            if (!Directory.Exists(serverConvertersFolder))
            {
                Directory.CreateDirectory(serverConvertersFolder);
            }
            await GenerateServerConvertersAsync(connector, contractType, serverConvertersFolder, name);
        }
        if (_configuration["controllers"] is string controllersFolder && !string.IsNullOrEmpty(controllersFolder))
        {
            if (!Directory.Exists(controllersFolder))
            {
                Directory.CreateDirectory(controllersFolder);
            }
            await GenerateControllerAsync(connector, contractType, controllersFolder, $"{name}Controller");
        }
        if (_configuration["services"] is string servicesFolder && !string.IsNullOrEmpty(servicesFolder))
        {
            if (!Directory.Exists(servicesFolder))
            {
                Directory.CreateDirectory(servicesFolder);
            }
            await GenerateServiceBaseAsync(connector, contractType, servicesFolder, $"{name}ServiceBase");
        }
        if (_configuration["connectors"] is string connectorsFolder && !string.IsNullOrEmpty(connectorsFolder))
        {
            if (!Directory.Exists(connectorsFolder))
            {
                Directory.CreateDirectory(connectorsFolder);
            }
            await GenerateConnectorAsync(connector, contractType, connectorsFolder, $"{name}Connector");
        }
        foreach (EntityAttribute entityAttribute in contractType.GetCustomAttributes<EntityAttribute>())
        {
            if (_configuration["server-models"] is string serverModelsFolder && !string.IsNullOrEmpty(serverModelsFolder))
            {
                if (!Directory.Exists(serverModelsFolder))
                {
                    Directory.CreateDirectory(serverModelsFolder);
                }
                await GenerateServerModelAsync(connector, contractType, entityAttribute.EntityType, serverModelsFolder);
            }
            if (_configuration["client-models"] is string clientModelsFolder && !string.IsNullOrEmpty(clientModelsFolder))
            {
                if (!Directory.Exists(clientModelsFolder))
                {
                    Directory.CreateDirectory(clientModelsFolder);
                }
                await GenerateClientModelAsync(connector, contractType, entityAttribute.EntityType, clientModelsFolder);
            }
        }
    }
    private async Task GenerateServerExtensionsAsync(IConnector connector, Type contractType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating server's extensions {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            ClassName = BuildResutTypeName(contractType, name),
        };
        await ProcessPageAsync(connector, options, "/Extensions", BuildFilePath(targetFolder, name));
        _logger?.LogInformation("Done.");
    }
    private async Task GenerateServerConvertersAsync(IConnector connector, Type contractType, string targetFolder, string name)
    {
        await GenerateServerConverterFactoryAsync(connector, contractType, targetFolder, $"{name}JsonConverterFactory");
        foreach (EntityAttribute attribute in contractType.GetCustomAttributes<EntityAttribute>())
        {
            string entityName = Util.GetTypeName(BuildResutTypeName(attribute.EntityType));
            await GenerateServerEntityConverterAsync(connector, contractType, attribute.EntityType, targetFolder, $"{entityName}JsonConverter");
        }
    }

    private async Task GenerateServerEntityConverterAsync(IConnector connector, Type contractType, Type entityType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating server's converter {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            EntityType = entityType,
            ClassName = BuildResutTypeName(entityType, name),
        };
        await ProcessPageAsync(connector, options, "/JsonConverter", BuildFilePath(targetFolder, name));
        _logger?.LogInformation("Done.");
    }

    private async Task GenerateServerConverterFactoryAsync(IConnector connector, Type contractType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating server's converter factory {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            ClassName = BuildResutTypeName(contractType, name),
        };
        await ProcessPageAsync(connector, options, "/JsonConverterFactory", BuildFilePath(targetFolder, name));
        _logger?.LogInformation("Done.");
    }

    private async Task GenerateConnectorAsync(IConnector connector, Type contractType, string targetFolder, string name)
    {
        switch (_clientLanguage)
        {
            case s_cSharp:
                await GenerateCSharpConnectorAsync(connector, contractType, targetFolder, name);
                break;
        }
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
    private async Task GenerateCSharpConnectorAsync(IConnector connector, Type contractType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating {lang} connector {name} at {folder}.", _clientLanguage, name, targetFolder);
        _logger?.LogInformation("Done.");
        await Task.CompletedTask;
    }
    private async Task GenerateCSharpClientModelAsync(IConnector connector, Type contractType, Type entityType, string targetFolder)
    {
        string name = BuildResutTypeName(entityType);
        _logger?.LogInformation("Generating {lang} client's entity model {name} at {folder}.", _clientLanguage, name, targetFolder);
        _logger?.LogInformation("Done.");
        await Task.CompletedTask;
    }
    private async Task GenerateContextAsync(IConnector connector, Type contractType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating context {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            ClassName = BuildResutTypeName(contractType, name),
        };
        await ProcessPageAsync(connector, options, "/Context", BuildFilePath(targetFolder, name));
        _logger?.LogInformation("Done.");
    }
    private async Task GenerateServerModelAsync(IConnector connector, Type contractType, Type entityType, string targetFolder)
    {
        string name = BuildResutTypeName(entityType);
        _logger?.LogInformation("Generating server's entity model {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            EntityType = entityType,
        };
        await ProcessPageAsync(connector, options, "/ServerModel", BuildFilePath(targetFolder, name));
        _logger?.LogInformation("Done.");
    }
    private async Task GenerateServiceBaseAsync(IConnector connector, Type contractType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating service base {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            ClassName = BuildResutTypeName(contractType, name),
        };
        await ProcessPageAsync(connector, options, "/ServiceBase", BuildFilePath(targetFolder, name));
        _logger?.LogInformation("Done.");
    }

    private async Task GenerateControllerAsync(IConnector connector, Type contractType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating controller {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            ClassName = BuildResutTypeName(contractType, name),
        };
        await ProcessPageAsync(connector, options, "/Controller", BuildFilePath(targetFolder, name));
        _logger?.LogInformation("Done.");
    }
    private static async Task ProcessPageAsync(IConnector connector, PageOptions? parameter, string uri, string path)
    {
        HttpRequestMessage request = new(HttpMethod.Get, uri);
        HttpResponseMessage response = connector.Send(request, parameter);
        Stream input = await response.Content.ReadAsStreamAsync();
        using FileStream output = new(path, FileMode.Create);
        await input.CopyToAsync(output);
    }
    private static string BuildResutTypeName(Type type, string? replaceName = null)
    {
        return $"{(string.IsNullOrEmpty(type.Namespace) ? string.Empty : $"{type.Namespace}.")}{(string.IsNullOrEmpty(replaceName) ? type.Name[1..] : replaceName)}";
    }
    private string BuildFilePath(string targetFolder, string name)
    {
        string typeName = Util.GetTypeName(name);
        return $"{Path.Combine(targetFolder, typeName)}{_fileExtension}";
    }
    private static string BuildTypeName(Type type, ClassModel model, HashSet<Type> entityTypes)
    {
        string result;
        if (entityTypes.Contains(type))
        {
            result = Util.GetTypeName(BuildResutTypeName(type));
        }
        else if (type.IsGenericType)
        {
            result = BuildGenericResultTypeName(type, model, entityTypes);
        }
        else
        {
            result = Util.BuildTypeName(type);
        }
        return result;
    }
    private static string BuildGenericResultTypeName(Type type, ClassModel model, HashSet<Type> entityTypes)
    {
        Type[] args = type.GetGenericArguments();
        StringBuilder sb = new(Util.BuildTypeName(type));
        int ltIndex = sb.ToString().IndexOf('<');
        sb.Remove(ltIndex + 1, sb.Length - ltIndex - 1);
        for (int i = 0; i < args.Length; ++i)
        {
            model.AddUsing(args[i]);
            if (i > 0)
            {
                sb.Append('.');
            }
            if (entityTypes.Contains(args[i]))
            {
                sb.Append(Util.GetTypeName(BuildResutTypeName(args[i])));
            }
            else
            {
                sb.Append(Util.BuildTypeName(args[i]));
            }
        }
        sb.Append('>');
        return sb.ToString();
    }
    private static string PascalCase(string folder)
    {
        StringBuilder sb = new();
        bool upper = true;
        foreach(char ch in folder)
        {
            if(upper)
            {
                sb.Append(new string([ch]).ToUpper());
                upper = false;
            }
            else if(ch == '-')
            {
                upper = true;
            }
            else
            {
                sb.Append(ch);
            }
        }
        return sb.ToString();
    }
    private static void Usage()
    {

    }
}