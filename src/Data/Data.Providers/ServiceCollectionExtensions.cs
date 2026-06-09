using Microsoft.Extensions.DependencyInjection;

namespace SanskritQuest.Main.Data.Providers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataProviders(this IServiceCollection services)
    {
        services.AddSingleton<DataService>();
        return services;
    }
}
