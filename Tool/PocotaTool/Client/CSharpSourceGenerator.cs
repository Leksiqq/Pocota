using Net.Leksi.E6dWebApp;
using Net.Leksi.Pocota.Client;
using Net.Leksi.Pocota.Contract;
using Net.Leksi.Pocota.Tool.Pages.Client.CS;
using Net.Leksi.Pocota.Tool.Pages.CS;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;
using Net.Leksi.Pocota.Server;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Net.Leksi.Pocota.Tool.Client;

internal class CSharpSourceGenerator: IClientSourceGenerator
{
    private const string s_sourceFileExtention = ".cs";
    private const string s_dependencyInjectionNs = "Microsoft.Extensions.DependencyInjection";
    private readonly IServiceProvider _services;
    private readonly ILogger<CSharpSourceGenerator>? _logger;

    internal CSharpSourceGenerator(IServiceProvider services)
    {
        _services = services;
        _logger = _services.GetService<ILogger<CSharpSourceGenerator>>();
    }

    public async Task GenerateModelAsync(IConnector connector, Type contractType, Type entityType, string targetFolder)
    {
        string name = Util.BuildTypeName(entityType);
        _logger?.LogInformation("Generating c# client's entity model {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            EntityType = entityType,
            ClassName = Util.BuildResutTypeName(entityType, name),
        };
        await Util.ProcessPageAsync(connector, options, "/Client/CS/Model", Util.BuildFilePath(targetFolder, name, s_sourceFileExtention));
        _logger?.LogInformation("Done.");
        name = $"{name}JsonConverter";
        options.ClassName = Util.BuildResutTypeName(entityType, name);
        targetFolder = Path.Combine(targetFolder, "..", "Converters");
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }
        _logger?.LogInformation("Generating c# converter {name} at {folder}.", name, targetFolder);
        await Util.ProcessPageAsync(connector, options, "/Client/CS/JsonConverter", Util.BuildFilePath(targetFolder, name, s_sourceFileExtention));
        _logger?.LogInformation("Done.");
        name = $"I{name.Replace("JsonConverter", "PocotaEntity")}";
        options.ClassName = Util.BuildResutTypeName(entityType, name);
        targetFolder = Path.Combine(targetFolder, "..", "PocotaEntities");
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }
        _logger?.LogInformation("Generating c# converter {name} at {folder}.", name, targetFolder);
        await Util.ProcessPageAsync(connector, options, "/Client/CS/IPocotaEntity", Util.BuildFilePath(targetFolder, name, s_sourceFileExtention));
        _logger?.LogInformation("Done.");
    }

    public async Task GenerateConnectorAsync(IConnector connector, Type contractType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating c# connector {name} at {folder}.", name, targetFolder);
        PageOptions options = new()
        {
            ContractType = contractType,
            ClassName = Util.BuildResutTypeName(contractType, name),
        };
        await Util.ProcessPageAsync(connector, options, "/Client/CS/Connector", Util.BuildFilePath(targetFolder, name, s_sourceFileExtention));
        _logger?.LogInformation("Done.");
        name = name.Replace("Connector", "JsonConverterFactory");
        options.ClassName = Util.BuildResutTypeName(contractType, name);
        targetFolder = Path.Combine(targetFolder, "..", "Converters");
        if(!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }
        _logger?.LogInformation("Generating c# converter factory {name} at {folder}.", name, targetFolder);
        await Util.ProcessPageAsync(connector, options, "/Client/CS/JsonConverterFactory", Util.BuildFilePath(targetFolder, name, s_sourceFileExtention));
        _logger?.LogInformation("Done.");
        name = name.Replace("JsonConverterFactory", "PocotaContext");
        options.ClassName = Util.BuildResutTypeName(contractType, name);
        targetFolder = Path.Combine(targetFolder, "..", "Context");
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }
        _logger?.LogInformation("Generating c# converter factory {name} at {folder}.", name, targetFolder);
        await Util.ProcessPageAsync(connector, options, "/Client/CS/PocotaContext", Util.BuildFilePath(targetFolder, name, s_sourceFileExtention));
        _logger?.LogInformation("Done.");
    }

    public async Task GenerateExtensionsAsync(IConnector connector, Type contractType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating c# extensions {name} at {folder}.", name, targetFolder);
        _logger?.LogInformation("Done.");
        await Task.CompletedTask;
    }
    internal static void PopulateClientModel(ModelModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = $"{Util.GetNamespace(options.ClassName!)}.Client";
        model.AddInheritance(typeof(Pocota.Client.PocotaEntity));
        model.AddInheritance($"I{model.ClassName}PocotaEntity");
        model.AddUsing(typeof(Pocota.Client.EntityProperty));
        NullabilityInfoContext nullabilityInfoContext = new();
        foreach (PropertyInfo pi in options.EntityType!.GetProperties())
        {
            model.AddUsing(pi.PropertyType);
            PropertyModel pm = new()
            {
                Name = pi.Name,
                TypeName = Util.BuildTypeName(pi.PropertyType),
                IsCollection = pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>),
                IsNullable = nullabilityInfoContext.Create(pi).ReadState is NullabilityState.Nullable,
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

    internal static void PopulateConnectorModel(ConnectorModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = $"{Util.GetNamespace(options.ClassName!)}.Client";
        model.ContractName = options.ContractType!.GetCustomAttribute<PocotaContractAttribute>()!.ContractName!;
        model.AddInheritance(typeof(Connector));
        model.AddUsing(typeof(IServiceProvider));
        model.AddUsing(typeof(SemaphoreSlim));
        model.AddUsing(s_dependencyInjectionNs);
        foreach (MethodInfo mi in options.ContractType!.GetMethods())
        {
            MethodModel mm = new()
            {
                Name = $"{mi.Name}Async",
                ReturnTypeName = Util.BuildTypeName(mi.ReturnType.IsGenericType ? typeof(Task) : typeof(ValueTask<>).MakeGenericType([mi.ReturnType])),
                IsEnumeration = mi.ReturnType.IsGenericType,
            };
            if(mm.IsEnumeration)
            {
                model.AddUsing(typeof(Task));
                Type itemType = mi.ReturnType.GetGenericArguments()[0];
                if (options.ContractType.GetCustomAttributes<EntityAttribute>().Where(a => a.EntityType.Name == itemType.Name).FirstOrDefault() is null)
                {
                    model.AddUsing(itemType);
                }
                else
                {
                    model.AddUsing($"{(string.IsNullOrEmpty(itemType.Namespace) ? string.Empty : $"{itemType.Namespace}.")}Client");
                }
                mm.Parameters.Add(new ParameterModel
                {
                    Name = "target",
                    TypeName = Util.BuildTypeName(typeof(ICollection<>).MakeGenericType([itemType])),
                });
            }
            else
            {
                model.AddUsing(typeof(ValueTask));
            }
            foreach (ParameterInfo pi in mi.GetParameters())
            {
                if (options.ContractType.GetCustomAttributes<EntityAttribute>().Where(a => a.EntityType.Name == pi.ParameterType.Name).FirstOrDefault() is null)
                {
                    model.AddUsing(pi.ParameterType);
                }
                else
                {
                    model.AddUsing($"{(string.IsNullOrEmpty(pi.ParameterType.Namespace) ? string.Empty : $"{pi.ParameterType.Namespace}.")}Client");
                }
                ParameterModel pm = new()
                {
                    Name = pi.Name!,
                    TypeName = Util.BuildTypeName(pi.ParameterType),
                };
                mm.Parameters.Add(pm);
            }
            model.Methods.Add(mm);
        }
    }

    internal static void PopulateJsonConverterFactoryModel(JsonConverterFactoryModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = $"{Util.GetNamespace(options.ClassName!)}.Client";
        model.AddUsing(typeof(HashSet<>));
        model.AddInheritance(typeof(JsonConverterFactory));
        model.AddUsing(typeof(JsonConverter));
        model.AddUsing(typeof(JsonSerializerOptions));
        model.AddUsing(typeof(IServiceProvider));
        model.AddUsing(s_dependencyInjectionNs);
        foreach (EntityAttribute attribute in options.ContractType!.GetCustomAttributes<EntityAttribute>())
        {
            model.AddUsing($"{(string.IsNullOrEmpty(attribute.EntityType.Namespace) ? string.Empty : $"{attribute.EntityType.Namespace}.")}Client");
            model.Entities.Add(Util.BuildTypeName(attribute.EntityType));
        }
    }

    internal static void PopulateJsonConverterModel(JsonConverterModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = $"{Util.GetNamespace(options.ClassName!)}.Client";
        model.AddUsing(typeof(JsonConverter<>));
        model.AddUsing(typeof(JsonSerializer));
        model.AddUsing(typeof(IServiceProvider));
        model.AddUsing(options.ContractType!);
        model.AddUsing(s_dependencyInjectionNs);
        model.EntityTypeName = Util.BuildTypeName(options.EntityType!);
        model.ContractName = options.ContractType!.GetCustomAttribute<PocotaContractAttribute>()!.ContractName!;
        model.AddInheritance(typeof(JsonConverter<>).MakeGenericType([options.EntityType!]));
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

    internal static void PopulatePocotaEntityModel(IPocotaEntityModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = $"{Util.GetNamespace(options.ClassName!)}.Client";
        model.ContractName = options.ContractType!.GetCustomAttribute<PocotaContractAttribute>()!.ContractName!;
        model.AddInheritance(typeof(IPocotaEntity));
        model.AddUsing(typeof(Pocota.Client.EntityProperty));
        model.AddUsing(typeof(IServiceProvider));
        foreach (PropertyInfo pi in options.EntityType!.GetProperties())
        {
            if (pi.GetCustomAttribute<JsonIgnoreAttribute>() is null)
            {
                PropertyModel pm = new()
                {
                    Name = pi.Name,
                };
                model.Properties.Add(pm);
            }
        }
    }

    internal static void PopulatePocotaContextModel(PocotaContextModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = $"{Util.GetNamespace(options.ClassName!)}.Client";
        model.AddInheritance(typeof(Pocota.Client.PocotaContext));
        model.AddUsing(typeof(HashSet<>));
        foreach (EntityAttribute attribute in options.ContractType!.GetCustomAttributes<EntityAttribute>())
        {
            model.AddUsing($"{(string.IsNullOrEmpty(attribute.EntityType.Namespace) ? string.Empty : $"{attribute.EntityType.Namespace}.")}Client");
            model.Entities.Add(Util.BuildTypeName(attribute.EntityType));
        }
    }
}
