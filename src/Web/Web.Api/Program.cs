using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SanskritQuest.Main.Web.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure WebRootPath pointing to Web.Client/dist or root dist for static React SPA assets.
        var contentRoot = builder.Environment.ContentRootPath;
        var webClientDist = Path.Combine(contentRoot, "..", "dist");
        if (!Directory.Exists(webClientDist))
        {
            webClientDist = Path.Combine(contentRoot, "..", "Web.Client", "dist");
        }
        if (!Directory.Exists(webClientDist))
        {
            webClientDist = Path.Combine(contentRoot, "..", "..", "..", "Web.Client", "dist");
        }
        if (!Directory.Exists(webClientDist))
        {
            webClientDist = Path.Combine(contentRoot, "wwwroot");
        }
        Console.WriteLine($"[Web.Api] React Static Web Root Path: {webClientDist}");
        builder.Environment.WebRootPath = webClientDist;

        var startup = new Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);

        var app = builder.Build();
        startup.Configure(app, app.Environment);

        app.Run();
    }
}
