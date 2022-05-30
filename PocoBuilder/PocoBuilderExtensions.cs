using Microsoft.Extensions.DependencyInjection;

namespace Net.Leksi.Pocota;

public static class PocoBuilderExtensions
{
    public static IServiceCollection AddPocoBuilder(this IServiceCollection services)
    {
        return services.AddTransient<PocoBuilder>();
    }
}
