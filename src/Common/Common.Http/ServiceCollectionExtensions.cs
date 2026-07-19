using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace SanskritQuest.Common.Http;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonHttp(this IServiceCollection services)
    {
        // 1. Register standard IHttpClientFactory with SocketsHttpHandler connection pooling optimization
        services.AddHttpClient();
        services.ConfigureHttpClientDefaults(builder =>
        {
            builder.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(15) // Keep connections open for 15 minutes, then refresh DNS
            });
        });

        // 2. Register custom interfaces
        services.AddSingleton<ICommonHttpClientFactory, CommonHttpClientFactory>();

        return services;
    }
}
