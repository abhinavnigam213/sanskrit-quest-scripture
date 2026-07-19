using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using Insight.Database;
using Insight.Database.Providers.PostgreSQL;
using Insight.Database.Serialization;
using Microsoft.Extensions.DependencyInjection;
using SanskritQuest.Common.Contracts;
using SanskritQuest.Data.Contracts;
using SanskritQuest.Data.Providers.Serialization;

namespace SanskritQuest.Data.Providers.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDataProviders(this IServiceCollection services)
	{
		PostgreSQLInsightDbProvider.RegisterProvider();

		var serializer = new SystemTextJsonInsightSerializer();
		DbSerializationRule.Serialize<LocalizedTitles>(serializer);
		DbSerializationRule.Serialize<LocalizedDescription>(serializer);
		DbSerializationRule.Serialize<List<DbWordBreakdownItem>>(serializer);
		DbSerializationRule.Serialize<VerseData>(serializer);

		services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
		services.AddSingleton<IScripturesDataProvider, ScripturesDataProvider>();
		services.AddSingleton<IDictionaryDataProvider, DictionaryDataProvider>();
		services.AddSingleton<ILocalDataSetsProvider, LocalDataSetsProvider>();
		return services;
	}
}
