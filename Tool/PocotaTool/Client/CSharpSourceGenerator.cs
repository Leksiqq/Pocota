using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Net.Leksi.E6dWebApp;
using Net.Leksi.Pocota.Client;
using Net.Leksi.Pocota.Contract;
using Net.Leksi.Pocota.Tool.Pages.Client.CS;
using Net.Leksi.Pocota.Tool.Pages.CS;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using System.Xml.Linq;

namespace Net.Leksi.Pocota.Tool.Client;

internal class CSharpSourceGenerator: IClientSourceGenerator
{
    private class EnvelopePageOptions: PageOptions
    {
        public HashSet<Type> Envelopes { get; set; }
        public HashSet<Type> Entities { get; set; }
    }
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
        _logger?.LogInformation("Generating c# pocota context {name} at {folder}.", name, targetFolder);
        await Util.ProcessPageAsync(connector, options, "/Client/CS/PocotaContext", Util.BuildFilePath(targetFolder, name, s_sourceFileExtention));
        _logger?.LogInformation("Done.");
        name = name.Replace("PocotaContext", "Extensions");
        options.ClassName = Util.BuildResutTypeName(contractType, name);
        targetFolder = Path.Combine(targetFolder, "..", "Extensions");
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }
        _logger?.LogInformation("Generating c# extensions {name} at {folder}.", name, targetFolder);
        await Util.ProcessPageAsync(connector, options, "/Client/CS/Extensions", Util.BuildFilePath(targetFolder, name, s_sourceFileExtention));
        _logger?.LogInformation("Done.");
    }
    public async Task GenerateEnvelopeAsync(IConnector connector, Type contractType, Type envelopeType, string targetFolder, HashSet<Type> envelopes, HashSet<Type> entities)
    {
        string name = Util.BuildTypeName(envelopeType);
        _logger?.LogInformation("Generating c# envelope {type} at {folder}.", envelopeType, targetFolder);
        EnvelopePageOptions options = new()
        {
            ContractType = contractType,
            EntityType = envelopeType,
            ClassName = Util.BuildResutTypeName(envelopeType, name),
            Entities = entities,
            Envelopes = envelopes
        };
        await Util.ProcessPageAsync(connector, options, "/Client/CS/Envelope", Util.BuildFilePath(targetFolder, name, s_sourceFileExtention));
        _logger?.LogInformation("Done.");
    }
    internal static void PopulateClientModel(ModelModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = BuildClientNamespace(options.ClassName!);
        model.AddInheritance(typeof(Pocota.Client.PocotaEntity));
        model.AddUsing(typeof(EntityProperty));
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
                pm.TypeName = Util.BuildTypeName(typeof(IList<>).MakeGenericType([itemType]));
            }
            model.Properties.Add(pm);
        }
    }
    internal static void PopulateConnectorModel(ConnectorModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = BuildClientNamespace(options.ClassName!);
        model.ContractName = options.ContractType!.GetCustomAttribute<PocotaContractAttribute>()!.ContractName!;
        model.JsonConverterFactoryClassName = $"{model.ContractName}JsonConverterFactory";
        model.AddInheritance(typeof(Connector));
        model.AddUsing(typeof(IServiceProvider));
        model.AddUsing(typeof(SemaphoreSlim));
        model.AddUsing(typeof(JsonSerializerOptions));
        model.AddUsing(s_dependencyInjectionNs);
        model.AddUsing(typeof(HttpUtility));
        foreach (MethodInfo mi in options.ContractType!.GetMethods())
        {
            MethodModel mm = new()
            {
                Name = $"{mi.Name}Async",
                ReturnTypeName = Util.BuildTypeName(mi.ReturnType.IsGenericType ? typeof(Task) : typeof(ValueTask<>).MakeGenericType([mi.ReturnType])),
                ReturnItemTypeName = Util.BuildTypeName(mi.ReturnType),
                IsEnumeration = mi.ReturnType.IsGenericType,
            };
            if (mm.IsEnumeration)
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
                    ItemTypeName = Util.BuildTypeName(itemType),
                });
            }
            else
            {
                mm.ReturnTypeName = mm.ReturnTypeName.Replace(">", "?>");
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
        model.NamespaceValue = BuildClientNamespace(options.ClassName!);
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
        model.NamespaceValue = BuildClientNamespace(options.ClassName!);
        model.AddUsing(typeof(JsonConverter<>));
        model.AddUsing(typeof(JsonSerializer));
        model.AddUsing(typeof(PocotaContext));
        model.AddUsing(typeof(IServiceProvider));
        model.AddUsing(options.ContractType!);
        model.AddUsing(s_dependencyInjectionNs);
        model.AddUsing($"{Util.GetNamespace(model.Contract!)}.Client");
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
        model.NamespaceValue = BuildClientNamespace(options.ClassName!);
        model.ContractName = options.ContractType!.GetCustomAttribute<PocotaContractAttribute>()!.ContractName!;
        model.AddInheritance(typeof(IPocotaEntity));
        model.AddUsing(typeof(EntityProperty));
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
        model.NamespaceValue = BuildClientNamespace(options.ClassName!);
        model.AddInheritance(typeof(PocotaContext));
        model.AddUsing(typeof(HashSet<>));
        foreach (EntityAttribute attribute in options.ContractType!.GetCustomAttributes<EntityAttribute>())
        {
            model.AddUsing($"{(string.IsNullOrEmpty(attribute.EntityType.Namespace) ? string.Empty : $"{attribute.EntityType.Namespace}.")}Client");
            model.Entities.Add(Util.BuildTypeName(attribute.EntityType));
        }
    }
    internal static void PopulateExtensionsModel(ExtensionsModel model)
    {
        PageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as PageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = BuildClientNamespace(options.ClassName!);
        model.ContractName = options.ContractType!.GetCustomAttribute<PocotaContractAttribute>()!.ContractName!;
        model.AddUsing(typeof(IServiceCollection));
        model.AddUsing(typeof(PocotaContext));
        foreach (EntityAttribute attribute in options.ContractType!.GetCustomAttributes<EntityAttribute>())
        {
            model.AddUsing($"{(string.IsNullOrEmpty(attribute.EntityType.Namespace) ? string.Empty : $"{attribute.EntityType.Namespace}.")}Client");
            PropertyModel pm = new()
            {
                Name = Util.BuildTypeName(attribute.EntityType),
            };
            model.Properties.Add(pm);
        }
    }
    internal static void PopulateClientEnvelope(EnvelopeModel model)
    {
        EnvelopePageOptions options = (model.HttpContext.RequestServices.GetRequiredService<RequestParameter>()?.Parameter as EnvelopePageOptions)!;
        model.Contract = Util.BuildTypeFullName(options.ContractType!);
        model.ClassName = Util.GetTypeName(options.ClassName!);
        model.NamespaceValue = BuildClientNamespace(options.ClassName!);
        NullabilityInfoContext nullabilityInfoContext = new();
        if(options.EntityType!.BaseType != typeof(object))
        {
            model.AddInheritance(BuildTypeName(options.EntityType!.BaseType!, options.Entities, options.Envelopes, model));
        }
        foreach(Type intf in options.EntityType!.GetInterfaces())
        {
            model.AddInheritance(intf);
        }
        foreach (PropertyInfo pi in options.EntityType!.GetProperties())
        {
            if (!pi.CanWrite || pi.SetMethod!.ReturnParameter.GetRequiredCustomModifiers().Contains(typeof(System.Runtime.CompilerServices.IsExternalInit)))
            {
                throw new Exception("Readonly property!");
            }
            model.AddUsing(pi.PropertyType);
            PropertyModel pm = new()
            {
                Name = pi.Name,
                TypeName = BuildTypeName(pi.PropertyType, options.Entities, options.Envelopes, model),
                IsNullable = nullabilityInfoContext.Create(pi).ReadState is NullabilityState.Nullable,
            };
            model.Properties.Add(pm);
        }
    }
    private static string BuildClientNamespace(string className)
    {
        string ns = Util.GetNamespace(className);
        return $"{ns}{(string.IsNullOrEmpty(ns) ? string.Empty : ".")}Client";
    }
    private static string BuildTypeName(Type type, HashSet<Type> entities, HashSet<Type> envelopes, EnvelopeModel model)
    {
        StringBuilder sb = new();
        if (type.IsGenericType)
        {
            if(type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return BuildTypeName(type.GetGenericArguments()[0], entities, envelopes, model);
            }
            model.AddUsing(type.GetGenericTypeDefinition());
            sb.Append(type.GetGenericTypeDefinition().Name[..type.GetGenericTypeDefinition().Name.IndexOf('`')]).Append('<')
                .Append(string.Join(',', type.GetGenericArguments().Select(t => BuildTypeName(t, entities, envelopes, model)))).Append('>');
        }
        else
        {
            if (envelopes.Contains(type) || entities.Contains(type))
            {
                model.AddUsing(BuildClientNamespace(Util.BuildTypeFullName(type)));
            }
            else
            {
                model.AddUsing(type);
            }
            sb.Append(type.Name);
        }
        return sb.ToString();
    }
}
