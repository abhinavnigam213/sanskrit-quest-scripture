using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SanskritQuest.Common.Configuration
{
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Binds an IConfiguration object to a concrete object and add it as a singleton.
		/// </summary>
		/// <typeparam name="TConfig"></typeparam>
		/// <param name="services"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
		public static TConfig BindSingleton<TConfig>(this IServiceCollection services, IConfiguration sectionConfiguration)
			where TConfig : class, new()
		{
			ArgumentNullException.ThrowIfNull(services);
			ArgumentNullException.ThrowIfNull(sectionConfiguration);

			var config = new TConfig();
			sectionConfiguration.Bind(config);
			services.AddSingleton(config);
			return config;
		}

		public static IServiceCollection AddCommonConfiguration(this IServiceCollection services, IConfiguration configuration)
		{
			services.BindSingleton<AuthSettings>(configuration.GetSection("AuthSettings"));
			services.BindSingleton<ConnectionStrings>(configuration.GetSection("ConnectionStrings"));
			return services;
		}
	}
}
