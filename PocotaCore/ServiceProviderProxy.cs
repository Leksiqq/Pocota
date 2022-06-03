using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Leksi.Pocota.Core;

internal class ServiceProviderProxy : IServiceProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Container _container;

    internal ServiceProviderProxy(IServiceProvider serviceProvider) => 
        (_serviceProvider, _container) = (serviceProvider, serviceProvider.GetRequiredService<Container>());

    public object? GetService(Type serviceType)
    {
        object? result = _serviceProvider?.GetService(serviceType);
        if (result is IServiceScopeFactory serviceScopeFactory && result is not ServiceScopeFactoryProxy)
        {
            result = new ServiceScopeFactoryProxy(serviceScopeFactory);
        } 
        else
        {
            _container.CreateKeyRing(result);
        }
        return result;
    }
}
