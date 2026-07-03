
using Insight.Database.Providers.PostgreSQL;
using Microsoft.Extensions.DependencyInjection;
using SanskritQuest.Data.Contracts;

namespace SanskritQuest.Data.Providers.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDataProviders(this IServiceCollection services)
	{
		PostgreSQLInsightDbProvider.RegisterProvider();
		services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
		services.AddSingleton<IDictionaryDataProvider, DictionaryDataProvider>();
		services.AddSingleton<ILocalDataSetsProvider, LocalDataSetsProvider>();
		return services;
	}
}
