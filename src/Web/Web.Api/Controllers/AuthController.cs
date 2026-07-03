using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SanskritQuest.Common.Security;

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
    private readonly JwtTokenProvider _jwtTokenProvider;

    public AuthController(JwtTokenProvider jwtTokenProvider)
    {
        _jwtTokenProvider = jwtTokenProvider;
    }

    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] TokenRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.ClientId) || string.IsNullOrEmpty(request.ClientSecret))
        {
            return BadRequest(new { error = "Client ID and Client Secret are required." });
        }

        var (tokenString, expiresAt, isValid) = _jwtTokenProvider.GenerateToken(request.ClientId, request.ClientSecret);

        if (!isValid || tokenString == null || expiresAt == null)
        {
            return Unauthorized(new { error = "Invalid Client ID or Client Secret configuration." });
        }

        return Ok(new TokenResponse(
            Token: tokenString,
            Type: "Bearer",
            ExpiresAt: expiresAt.Value
        ));
    }
}
