using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SanskritQuest.Common.Configuration;

namespace SanskritQuest.Common.Security
{
	public class JwtTokenProvider
	{
		private readonly AuthSettings _authSettings;

		public JwtTokenProvider(AuthSettings jwtSettings)
		{
			_authSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
		}

		public (string? Token, DateTime? ExpiresAt, bool IsValid) GenerateToken(string clientId, string clientSecret)
		{
			var isValid = _authSettings.Clients.Exists(client => !string.IsNullOrEmpty(client.ClientId) &&
						!string.IsNullOrEmpty(client.ClientSecret) &&
						clientId == client.ClientId &&
						clientSecret == client.ClientSecret);

			if (!isValid)
			{
				return (null, null, false);
			}

			var jwtKey = _authSettings.JwtKey;
			if (string.IsNullOrEmpty(jwtKey))
			{
				jwtKey = "MySuperLongSecureDummySecurityKeyThatMustBeGreater32Bytes!";
			}

			var issuer = string.IsNullOrEmpty(_authSettings.JwtIssuer) ? "SanskritQuest.Web.Api" : _authSettings.JwtIssuer;
			var audience = string.IsNullOrEmpty(_authSettings.JwtAudience) ? "SanskritQuestApp" : _authSettings.JwtAudience;
			var expiryMinutes = _authSettings.ExpiryMinutes > 0 ? _authSettings.ExpiryMinutes : 120;

			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, clientId),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim("scope", "scriptures.read scriptures.write")
			};

			var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

			var token = new JwtSecurityToken(
				issuer: issuer,
				audience: audience,
				claims: claims,
				expires: expiresAt,
				signingCredentials: credentials);

			var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

			return (tokenString, expiresAt, true);
		}
	}
}
