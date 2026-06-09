using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;
using SanskritQuest.Main.Web.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Use URLs from launchSettings.json (https://localhost:5001, http://localhost:5000)
// No hardcoded Kestrel configuration - let launchSettings handle it

// 1. Configure and Register AI Services (Gemini ChatClient)
var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
if (!string.IsNullOrEmpty(apiKey))
{
    try
    {
        var clientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri("https://generativelanguage.googleapis.com/v1beta/openai/")
        };
        var openAIClient = new OpenAIClient(new ApiKeyCredential(apiKey), clientOptions);
        IChatClient chatClient = openAIClient.AsChatClient("gemini-1.5-flash");
        builder.Services.AddSingleton<IChatClient>(chatClient);
        Console.WriteLine("[Web.Api] Registered Gemini ChatClient successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Web.Api] Warning: Failed to build AI ChatClient: {ex.Message}");
    }
}
else
{
    Console.WriteLine("[Web.Api] Warning: GEMINI_API_KEY is not defined. Server starting in OFFLINE fallback mode.");
}

// Register internal state/computation services
builder.Services.AddSingleton<DataService>();
// AIService depends optionally on IChatClient. Use factory to allow null when chat client isn't registered.
builder.Services.AddSingleton<AIService>(sp => new AIService(sp.GetService<Microsoft.Extensions.AI.IChatClient>(), sp.GetRequiredService<DataService>()));

// 2. Configure JWT Authentication Services
var jwtKey = builder.Configuration["AuthSettings:JwtKey"] ?? "SanskritQuest3.5SuperSecureJWTTokenKeyDoubleStrength999!!!";
var issuer = builder.Configuration["AuthSettings:JwtIssuer"] ?? "SanskritQuest.Main.Web.Api";
var audience = builder.Configuration["AuthSettings:JwtAudience"] ?? "SanskritQuestApp";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// 3. Register Controllers and configure strict JSON formatting
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// 4. Configure CORS Policies
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 5. Configure Swagger and OpenAPI generation with security description
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SanskritQuest API Portal",
        Version = "v1",
        Description = "An advanced philological engine, scripture dictionary, translation center, and sandhi-analyser representing scriptures (Vedas, Upanishads, Gita, Ramayana, Puranas)."
    });

    // Provide a beautiful Bearer token description for the Swagger user interface
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer [space] <your token>' in the field below.\n\nExample: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            Array.Empty<string>()
        }
    });
});

// Configure WebRootPath pointing to Web.Client/dist or root dist for static React SPA assets.
var webClientDist = Path.Combine(builder.Environment.ContentRootPath, "..", "dist");
if (!Directory.Exists(webClientDist))
{
    webClientDist = Path.Combine(builder.Environment.ContentRootPath, "..", "Web.Client", "dist");
}
if (!Directory.Exists(webClientDist))
{
    webClientDist = Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
}
Console.WriteLine($"[Web.Api] React Static Web Root Path: {webClientDist}");
builder.Environment.WebRootPath = webClientDist;

var app = builder.Build();

// Enable CORS
app.UseCors();

// Serve React SPA static files
app.UseDefaultFiles();
app.UseStaticFiles();

// Enable Swagger UI and JSON docs for local exploration / client reference
app.UseSwagger(options =>
{
    // Keeping OpenAPI paths predictable
    options.RouteTemplate = "swagger/{documentName}/swagger.json";
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SanskritQuest API Portal v1");
    // Served at the '/swagger' route
    c.RoutePrefix = "swagger";
});

// Enable Authentication and Authorization triggers
app.UseAuthentication();
app.UseAuthorization();

// Setup debug request telemetry
app.Use(async (context, next) =>
{
    Console.WriteLine($"[{DateTime.UtcNow:o}] C# controller call: {context.Request.Method} {context.Request.Path}");
    await next();
});

// Register controllers endpoints
app.MapControllers();

// Serve index.html for client SPA routes
app.MapFallbackToFile("index.html");

// Simple API status reporting fallback
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", webapi = "dotnet-controller-mode", authenticatedWithJwt = true }));

app.Run();
