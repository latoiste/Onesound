using OneSound.Endpoints;
using OneSound.Media.Handler;
using OneSound.Services;
using OneSound.Media.Session;
using WindowsMediaController;

namespace OneSound;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<MediaManager>();
        builder.Services.AddSingleton<SessionManager>();
        builder.Services.AddSingleton<MediaEventHandler>();
        
        builder.Services.AddHostedService<MediaManagerService>();

        var app = builder.Build();

        app.MapGroup("/media")
            .MapMediaApi();
        app.MapGet("/health", () => Results.Ok());
        app.Run();
    }
}