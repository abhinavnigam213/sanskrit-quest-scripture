using Microsoft.Extensions.DependencyInjection;

namespace SanskritQuest.Common.Security
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddCommonSecurity(this IServiceCollection services)
		{
			services.AddSingleton<JwtTokenProvider>();
			return services;
		}
	}
}
