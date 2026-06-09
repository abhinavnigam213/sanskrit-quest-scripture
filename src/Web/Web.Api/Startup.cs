using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SanskritQuest.Main.Data.Providers;
using SanskritQuest.Main.Services.AIService;
using SanskritQuest.Main.Business.Providers;

namespace SanskritQuest.Main.Web.Api;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // 1. Register Layer Dependencies using Extension Methods
        services.AddDataProviders();
        services.AddAIServices();
        services.AddBusinessProviders();

        // 2. Configure JWT Authentication Services
        var jwtKey = Configuration["AuthSettings:JwtKey"] ?? "SanskritQuest3.5SuperSecureJWTTokenKeyDoubleStrength999!!!";
        var issuer = Configuration["AuthSettings:JwtIssuer"] ?? "SanskritQuest.Main.Web.Api";
        var audience = Configuration["AuthSettings:JwtAudience"] ?? "SanskritQuestApp";

        services.AddAuthentication(options =>
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
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

        // 4. Configure CORS Policies
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        // 5. Configure Swagger and OpenAPI generation with security description
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SanskritQuest API Portal",
                Version = "v1",
                Description = "An advanced philological engine, scripture dictionary, translation center, and sandhi-analyser representing scriptures (Vedas, Upanishads, Gita, Ramayana, Puranas)."
            });

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
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Serve React SPA static files
        app.UseDefaultFiles();
        app.UseStaticFiles();

        // Enable CORS
        app.UseCors();

        // Enable Swagger UI and JSON docs for local exploration
        app.UseSwagger(options =>
        {
            options.RouteTemplate = "swagger/{documentName}/swagger.json";
        });
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "SanskritQuest API Portal v1");
            c.RoutePrefix = "swagger";
        });

        // Enable Authentication and Authorization triggers
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        // Setup debug request telemetry
        app.Use(async (context, next) =>
        {
            Console.WriteLine($"[{DateTime.UtcNow:o}] C# controller call: {context.Request.Method} {context.Request.Path}");
            await next();
        });

        // Register controller endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("index.html");
        });
    }
}
