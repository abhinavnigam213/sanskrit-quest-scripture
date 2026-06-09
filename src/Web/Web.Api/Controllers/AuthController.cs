using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace SanskritQuest.Web.Api.Controllers;

public record TokenRequest(
    [property: JsonPropertyName("clientId")] string ClientId,
    [property: JsonPropertyName("clientSecret")] string ClientSecret
);

public record TokenResponse(
    [property: JsonPropertyName("token")] string Token,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("expiresAt")] DateTime ExpiresAt
);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] TokenRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.ClientId) || string.IsNullOrEmpty(request.ClientSecret))
        {
            return BadRequest(new { error = "Client ID and Client Secret are required." });
        }

        var expectedClientId = _configuration["AuthSettings:ClientId"];
        var expectedClientSecret = _configuration["AuthSettings:ClientSecret"];

        bool isValid = false;
        if (!string.IsNullOrEmpty(expectedClientId) && request.ClientId == expectedClientId && request.ClientSecret == expectedClientSecret)
        {
            isValid = true;
        }
        else
        {
            var clientsSection = _configuration.GetSection("AuthSettings:Clients");
            if (clientsSection.Exists())
            {
                foreach (var client in clientsSection.GetChildren())
                {
                    var cid = client["ClientId"];
                    var csec = client["ClientSecret"];
                    if (!string.IsNullOrEmpty(cid) && !string.IsNullOrEmpty(csec) && request.ClientId == cid && request.ClientSecret == csec)
                    {
                        isValid = true;
                        break;
                    }
                }
            }
        }

        if (!isValid)
        {
            return Unauthorized(new { error = "Invalid Client ID or Client Secret configuration." });
        }

        var jwtKey = _configuration["AuthSettings:JwtKey"] ?? "MySuperLongSecureDummySecurityKeyThatMustBeGreater32Bytes!";
        var issuer = _configuration["AuthSettings:JwtIssuer"] ?? "SanskritQuest.Web.Api";
        var audience = _configuration["AuthSettings:JwtAudience"] ?? "SanskritQuestApp";
        var expiryMinutesDefault = 120;
        
        if (!int.TryParse(_configuration["AuthSettings:ExpiryMinutes"], out var expiryMinutes))
        {
            expiryMinutes = expiryMinutesDefault;
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.ClientId),
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

        return Ok(new TokenResponse(
            Token: tokenString,
            Type: "Bearer",
            ExpiresAt: expiresAt
        ));
    }
}
