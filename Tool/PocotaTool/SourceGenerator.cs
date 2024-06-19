using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using Net.Leksi.E6dWebApp;
using Net.Leksi.Pocota.Contract;
using Net.Leksi.Pocota.Server;
using Net.Leksi.Pocota.Tool;
using Net.Leksi.Pocota.Tool.Client;
using Net.Leksi.Pocota.Tool.Pages;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Net.Leksi.Pocota.Tool.Constants;

namespace Net.Leksi.Pocota;
public class SourceGenerator : Runner, ICommand
{
    private readonly IServiceProvider _services;
    private readonly ILogger<SourceGenerator>? _logger;
    private readonly IConfiguration _configuration;
    private readonly string[] _serverFolders = ["db-contexts", "controllers", "services", "server-converters", "server-extensions", "access", "pocota-entities"];
    private readonly string[] _clientFolders = ["connectors", "client-models", "client-envelopes", "client-converters", "client-extensions"];
    private readonly string _clientLanguage = s_cSharp;
    private readonly string _fileExtension = s_cs;
    private readonly HashSet<Type> _entityTypes = [];
    private readonly HashSet<Type> _envelopeTypes = [];
    internal IClientSourceGenerator? ClientSourceGenerator { get; private init; }
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
            }
        }
        if (_configuration["server-base"] is string sb && !string.IsNullOrEmpty(sb))
        {
            foreach (string folder in _serverFolders)
            {
                if (_configuration[folder] is null)
                {
                    _configuration[folder] = Path.Combine(sb, Util.PascalCase(folder.StartsWith("server-") ? folder["server-".Length..] : folder));
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
            switch (_clientLanguage)
            {
                case s_cSharp:
                    _fileExtension = s_cs;
                    ClientSourceGenerator = new CSharpSourceGenerator(_services);
                    break;
            }
            foreach (string folder in _clientFolders)
            {
                if (_configuration[folder] is null)
                {
                    _configuration[folder] = Path.Combine(cb, Util.PascalCase(folder.StartsWith("client-") ? folder["client-".Length..] : folder));
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
                await ProcessContractAsync(connector, type, pca.ContractName ?? Util.BuildResutTypeName(type));
                _logger?.LogInformation("Done processing contract type.");
            }
        }
    }
    internal static void PopulateContextModel(DbContextModel model)
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
                    : $"SetOf{attribute.EntityType.Name}",
                TypeName = attribute.EntityType.Name,
            };
            model.Properties.Add(pm);
        }
    }
    internal static void PopulateServiceBaseModel(ServiceBaseModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = Util.GetNamespace(options.ClassName!);
        foreach(MethodInfo mi in options.ContractType!.GetMethods())
        {
            model.AddUsing(mi.ReturnType);
            string returnTypeName = Util.BuildTypeName(mi.ReturnType);
            if (mi.ReturnType.IsGenericType)
            {
                model.AddUsing(typeof(IAsyncEnumerable<>));
                returnTypeName = returnTypeName.Replace(nameof(IEnumerable<object>), nameof(IAsyncEnumerable<object>));
            }
            else
            {
                model.AddUsing(typeof(ValueTask<>));
                returnTypeName = $"{nameof(ValueTask<object>)}<{returnTypeName}?>";
            }
            MethodModel mm = new()
            {
                Name = $"{mi.Name}Async",
                ReturnTypeName = returnTypeName,
            };
            foreach(ParameterInfo pi in mi.GetParameters())
            {
                model.AddUsing(pi.ParameterType);
                ParameterModel pm = new()
                {
                    Name = pi.Name!,
                    TypeName = pi.ParameterType.Name,
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
        model.ContractName = options.ContractType!.GetCustomAttribute<PocotaContractAttribute>()!.ContractName!;
        model.JsonConverterFactoryClassName = model.ClassName.Replace("Controller", "JsonConverterFactory");
        model.AddInheritance(typeof(ControllerBase));
        model.AddUsing(typeof(JsonSerializer));
        model.AddUsing(typeof(JsonSerializerOptions));
        model.AddUsing(typeof(ReferenceHandler));
        model.AddUsing(typeof(ApiControllerAttribute));
        model.AddUsing(typeof(RouteAttribute));
        model.AddUsing(typeof(HttpGetAttribute));
        model.AddUsing(typeof(HttpPostAttribute));
        model.AddUsing(typeof(PocotaContext));
        foreach (MethodInfo mi in options.ContractType!.GetMethods())
        {
            model.AddUsing(mi.ReturnType);
            MethodModel mm = new()
            {
                Name = mi.Name,
                ReturnTypeName = mi.ReturnType.IsGenericType ? mi.ReturnType.GetGenericArguments()[0].Name : mi.ReturnType.Name,
                IsEnumeration = mi.ReturnType.IsGenericType,
            };
            mm.Attributes.Add($"[HttpGet(\"{mi.Name}{(mi.GetParameters().Length > 0 ? $"/{string.Join('/', mi.GetParameters().Select(p => $"{{{p.Name}}}"))}" : string.Empty)}\")]");
            foreach (ParameterInfo pi in mi.GetParameters())
            {
                model.AddUsing(pi.ParameterType);
                ParameterModel pm = new()
                {
                    Name = pi.Name!,
                    TypeName = pi.ParameterType.Name,
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
            model.Entities.Add(Util.BuildTypeName(attribute.EntityType));
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
        model.AddUsing(typeof(PocotaContext));
        model.AddUsing(options.ContractType!);
        model.AddUsing(typeof(IHttpContextAccessor));
        model.AddUsing(typeof(PocotaHeader));
        model.EntityTypeName = Util.BuildTypeName(options.EntityType!);
        model.ContractName = options.ContractType!.GetCustomAttribute<PocotaContractAttribute>()!.ContractName!;
        model.AddInheritance(typeof(JsonConverter<>).MakeGenericType([options.EntityType!]));
        foreach (PropertyInfo pi in options.EntityType!.GetProperties())
        {
            if(pi.GetCustomAttribute<JsonIgnoreAttribute>() is null)
            {
                model.AddUsing(pi.PropertyType);
                PropertyModel pm = new()
                {
                    Name = pi.Name,
                };
                model.Properties.Add(pm);
            }
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
        model.AddUsing(typeof(PocotaContext));
        model.AddUsing(typeof(ServiceDescriptor));
        foreach (EntityAttribute attribute in options.ContractType!.GetCustomAttributes<EntityAttribute>())
        {
            model.AddUsing(attribute.EntityType);
            PropertyModel pm = new()
            {
                Name = Util.BuildTypeName(attribute.EntityType),
            };
            model.Properties.Add(pm);
        }
    }
    internal static void PopulateAccessModel(AccessModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = Util.GetNamespace(options.ClassName!);
        model.EntityTypeName = Util.BuildTypeName(options.EntityType!);
        model.ContractName = options.ContractType!.GetCustomAttribute<PocotaContractAttribute>()!.ContractName!;
        model.AddUsing(typeof(PocotaContext));
        model.AddUsing(typeof(ReferenceEntry));
        model.AddUsing(typeof(CollectionEntry));
        model.AddUsing(typeof(EntityEntry));
        model.AddUsing(typeof(AccessKind));
        model.AddInheritance(typeof(IAccessCalculator));
        foreach (PropertyInfo pi in options.EntityType!.GetProperties())
        {
            if(!pi.PropertyType.IsPrimitive && !pi.PropertyType.IsValueType && pi.PropertyType != typeof(string))
            {
                model.AddUsing(pi.PropertyType);
                PropertyModel pm = new()
                {
                    Name = pi.Name,
                    TypeName = pi.PropertyType.Name,
                    IsCollection = pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
                };
                if (pm.IsCollection)
                {
                    Type itemType = pi.PropertyType.GetGenericArguments()[0];
                    model.AddUsing(itemType);
                    pm.ItemTypeName = Util.BuildTypeName(itemType);
                }
                model.Properties.Add(pm);
            }
        }
    }
    internal static void PopulatePocotaEntityModel(PocotaEntityModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = Util.GetNamespace(options.ClassName!);
        model.ContractName = options.ContractType!.GetCustomAttribute<PocotaContractAttribute>()!.ContractName!;
        model.AddInheritance(typeof(PocotaEntity));
        model.AddUsing(typeof(EntityEntry));
        foreach (PropertyInfo pi in options.EntityType!.GetProperties())
        {
            if (pi.GetCustomAttribute<JsonIgnoreAttribute>() is null)
            {
                model.AddUsing(pi.PropertyType);
                PropertyModel pm = new()
                {
                    Name = pi.Name,
                };
                model.Properties.Add(pm);
            }
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
        CheckContract(contractType);
        if (!string.IsNullOrEmpty(_configuration["server-base"]))
        {
            if (_configuration["db-contexts"] is string dbContextsFolder && !string.IsNullOrEmpty(dbContextsFolder))
            {
                if (!Directory.Exists(dbContextsFolder))
                {
                    Directory.CreateDirectory(dbContextsFolder);
                }
                await GenerateDbContextAsync(connector, contractType, dbContextsFolder, $"{name}DbContext");
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
                await GenerateServerConverterFactoryAsync(connector, contractType, serverConvertersFolder, $"{name}JsonConverterFactory");
                foreach (EntityAttribute attribute in contractType.GetCustomAttributes<EntityAttribute>())
                {
                    string entityName = Util.BuildTypeName(attribute.EntityType);
                    await GenerateServerEntityConverterAsync(connector, contractType, attribute.EntityType, serverConvertersFolder, $"{entityName}JsonConverter");
                }
            }
            if (_configuration["access"] is string accessFolder && !string.IsNullOrEmpty(accessFolder))
            {
                if (!Directory.Exists(accessFolder))
                {
                    Directory.CreateDirectory(accessFolder);
                }
                foreach (EntityAttribute attribute in contractType.GetCustomAttributes<EntityAttribute>())
                {
                    string entityName = Util.BuildTypeName(attribute.EntityType);
                    await GenerateModelAccessAsync(connector, contractType, attribute.EntityType, accessFolder, $"{entityName}AccessBase");
                }
            }
            if (_configuration["pocota-entities"] is string pocotaEntitiesFolder && !string.IsNullOrEmpty(pocotaEntitiesFolder))
            {
                if (!Directory.Exists(pocotaEntitiesFolder))
                {
                    Directory.CreateDirectory(pocotaEntitiesFolder);
                }
                foreach (EntityAttribute attribute in contractType.GetCustomAttributes<EntityAttribute>())
                {
                    string entityName = Util.BuildTypeName(attribute.EntityType);
                    await GenerateModelPocotaEntityAsync(connector, contractType, attribute.EntityType, pocotaEntitiesFolder, $"{entityName}PocotaEntity");
                }
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
        }
        if (ClientSourceGenerator is IClientSourceGenerator clientSourceGenerator)
        {
            if (_configuration["connectors"] is string connectorsFolder && !string.IsNullOrEmpty(connectorsFolder))
            {
                if (!Directory.Exists(connectorsFolder))
                {
                    Directory.CreateDirectory(connectorsFolder);
                }
                    await clientSourceGenerator.GenerateConnectorAsync(connector, contractType, connectorsFolder, $"{name}Connector", _envelopeTypes, _entityTypes);
            }
            foreach (Type entityType in _entityTypes)
            {
                if (_configuration["client-models"] is string clientModelsFolder && !string.IsNullOrEmpty(clientModelsFolder))
                {
                    if (!Directory.Exists(clientModelsFolder))
                    {
                        Directory.CreateDirectory(clientModelsFolder);
                    }
                    await clientSourceGenerator.GenerateModelAsync(connector, contractType, entityType, clientModelsFolder);
                }
            }
            foreach(Type envelopeType in _envelopeTypes)
            {
                if (_configuration["client-envelopes"] is string clientEnvelopesFolder && !string.IsNullOrEmpty(clientEnvelopesFolder))
                {
                    if (!Directory.Exists(clientEnvelopesFolder))
                    {
                        Directory.CreateDirectory(clientEnvelopesFolder);
                    }
                    await clientSourceGenerator.GenerateEnvelopeAsync(connector, contractType, envelopeType, clientEnvelopesFolder, _envelopeTypes, _entityTypes);
                }
            }
        }
    }
    private bool IsSupportedType(Type type, bool allowGenerics, bool allowEnvelopes)
    {
        return type.IsPrimitive
        || type.IsEnum
        || SupportedTypes.Types.Contains(type)
        || (
            !allowEnvelopes
            && _entityTypes.Contains(type)
        )
        || (
            allowEnvelopes
            && (
                _entityTypes.Contains(type)
                || _envelopeTypes.Contains(type)
            )
        )
        || (
            allowGenerics
            && type.IsGenericType
            && (
                type.GetGenericTypeDefinition() == typeof(Nullable<>)
                || type.GetGenericTypeDefinition() == typeof(ICollection<>)
            )
            && type.GetGenericArguments()[0] is Type type1
            && IsSupportedType(type1, false, allowEnvelopes)
        );
    }
    private void CheckContract(Type contractType)
    {
        _entityTypes.Clear();
        _envelopeTypes.Clear();
        foreach (EntityAttribute entityAttribute in contractType.GetCustomAttributes<EntityAttribute>())
        {
            _entityTypes.Add(entityAttribute.EntityType);
        }
        foreach (EnvelopeAttribute envelopeAttribute in contractType.GetCustomAttributes<EnvelopeAttribute>())
        {
            _envelopeTypes.Add(envelopeAttribute.EnvelopeType);
        }
        foreach (Type type in _entityTypes)
        {
            foreach (PropertyInfo pi in type.GetProperties())
            {
                if (!IsSupportedType(pi.PropertyType, true, false))
                {
                    throw new Exception($"Entiti'es property {pi.PropertyType} {type}.{pi.Name} has unsupported type!");
                }
            }
        }
        foreach (Type type in _envelopeTypes)
        {
            foreach (PropertyInfo pi in type.GetProperties())
            {
                if (!IsSupportedType(pi.PropertyType, true, false))
                {
                    throw new Exception($"Envelope's property {pi.PropertyType} {type}.{pi.Name} has unsupported type!");
                }
            }
        }
        foreach(MethodInfo mi in contractType.GetMethods())
        {
            if (!IsSupportedReturnType(mi.ReturnType))
            {
                throw new Exception($"Method's {mi} return type is unsupported!");
            }
            foreach(ParameterInfo par in mi.GetParameters())
            {
                if (!IsSupportedParameterType(par.ParameterType))
                {
                    throw new Exception($"Method's {mi} parameter {par.Name} has unsupported type!");
                }
            }
        }
    }
    private bool IsSupportedParameterType(Type parameterType)
    {
        return IsSupportedType(parameterType, true, true) 
            && (
                !parameterType.IsGenericType
                || parameterType.GetGenericTypeDefinition() != typeof(ICollection<>)
            );
    }
    private bool IsSupportedReturnType(Type returnType)
    {
        return 
            returnType == typeof(void)
            || (
                IsSupportedType(returnType, true, true)
                && (
                    !returnType.IsGenericType
                    || returnType.GetGenericTypeDefinition() != typeof(ICollection<>)
                )
            )
            || (
                returnType.IsGenericType
                && returnType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                && returnType.GetGenericArguments()[0] is Type type1
                && IsSupportedType(type1, false, true)
            );
    }
    private async Task GenerateModelPocotaEntityAsync(IConnector connector, Type contractType, Type entityType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating model's pocota {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            EntityType = entityType,
            ClassName = Util.BuildResutTypeName(entityType, name),
        };
        await Util.ProcessPageAsync(connector, options, "/PocotaEntity", Util.BuildFilePath(targetFolder, name, _fileExtension));
        _logger?.LogInformation("Done.");
    }
    private async Task GenerateModelAccessAsync(IConnector connector, Type contractType, Type entityType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating model's access {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            EntityType = entityType,
            ClassName = Util.BuildResutTypeName(entityType, name),
        };
        await Util.ProcessPageAsync(connector, options, "/Access", Util.BuildFilePath(targetFolder, name, _fileExtension));
        _logger?.LogInformation("Done.");
    }

    private async Task GenerateServerExtensionsAsync(IConnector connector, Type contractType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating server's extensions {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            ClassName = Util.BuildResutTypeName(contractType, name),
        };
        await Util.ProcessPageAsync(connector, options, "/Extensions", Util.BuildFilePath(targetFolder, name, _fileExtension));
        _logger?.LogInformation("Done.");
    }
    private async Task GenerateServerEntityConverterAsync(IConnector connector, Type contractType, Type entityType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating server's converter {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            EntityType = entityType,
            ClassName = Util.BuildResutTypeName(entityType, name),
        };
        await Util.ProcessPageAsync(connector, options, "/JsonConverter", Util.BuildFilePath(targetFolder, name, _fileExtension));
        _logger?.LogInformation("Done.");
    }
    private async Task GenerateServerConverterFactoryAsync(IConnector connector, Type contractType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating server's converter factory {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            ClassName = Util.BuildResutTypeName(contractType, name),
        };
        await Util.ProcessPageAsync(connector, options, "/JsonConverterFactory", Util.BuildFilePath(targetFolder, name, _fileExtension));
        _logger?.LogInformation("Done.");
    }
    private async Task GenerateDbContextAsync(IConnector connector, Type contractType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating context {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            ClassName = Util.BuildResutTypeName(contractType, name),
        };
        await Util.ProcessPageAsync(connector, options, "/DbContext", Util.BuildFilePath(targetFolder, name, _fileExtension));
        _logger?.LogInformation("Done.");
    }
    private async Task GenerateServiceBaseAsync(IConnector connector, Type contractType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating service base {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            ClassName = Util.BuildResutTypeName(contractType, name),
        };
        await Util.ProcessPageAsync(connector, options, "/ServiceBase", Util.BuildFilePath(targetFolder, name, _fileExtension));
        _logger?.LogInformation("Done.");
    }
    private async Task GenerateControllerAsync(IConnector connector, Type contractType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating controller {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            ClassName = Util.BuildResutTypeName(contractType, name),
        };
        await Util.ProcessPageAsync(connector, options, "/Controller", Util.BuildFilePath(targetFolder, name, _fileExtension));
        _logger?.LogInformation("Done.");
    }
    private static void Usage()
    {

    }
}