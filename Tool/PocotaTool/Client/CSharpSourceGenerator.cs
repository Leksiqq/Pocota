using Microsoft.Extensions.DependencyInjection;
using Net.Leksi.E6dWebApp;
using Net.Leksi.Pocota.Contract;
using Net.Leksi.Pocota.Tool.Pages.Client.CS;
using System;
using System.Reflection;

namespace Net.Leksi.Pocota.Tool.Client;

internal class CSharpSourceGenerator: IClientSourceGenerator
{
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
        await Util.ProcessPageAsync(connector, options, "/Client/CS/Model", Util.BuildFilePath(targetFolder, name, ".cs"));
        _logger?.LogInformation("Done.");
        await Task.CompletedTask;
    }

    public async Task GenerateConnectorAsync(IConnector connector, Type contractType, string targetFolder, string name)
    {
        _logger?.LogInformation("Generating c# connector {name} at {folder}.", name, targetFolder);
        _logger?.LogInformation("Done.");
        await Task.CompletedTask;
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
        model.NamespaceValue = Util.GetNamespace(options.ClassName!);
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
}
