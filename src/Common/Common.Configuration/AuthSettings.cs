namespace SanskritQuest.Common.Configuration
{
	public class AuthSettings
	{
		public string JwtKey { get; set; } = string.Empty;
		public string JwtIssuer { get; set; } = string.Empty;
		public string JwtAudience { get; set; } = string.Empty;
		public int ExpiryMinutes { get; set; } = 60;
		public List<JwtClientSettings> Clients { get; set; } = [];
	}

	public class JwtClientSettings
	{
		public string ClientId { get; set; } = string.Empty;
		public string ClientSecret { get; set; } = string.Empty;
	}
}
