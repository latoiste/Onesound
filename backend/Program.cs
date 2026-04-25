using OneSound.Endpoints;
using OneSound.Media.Handler;
using OneSound.Services;
using OneSound.Media.Session;
using OneSound.Media.Manager;

namespace OneSound;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        #if WINDOWS
            builder.Services.AddSingleton<MediaManager, MediaManagerWindows>();
        #elif LINUX
            builder.Services.AddSingleton<MediaManager, MediaManagerLinux>();
        #endif

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