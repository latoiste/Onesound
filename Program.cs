using OneSound.Services;

namespace OneSound;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHostedService<MediaManagerService>();

        var app = builder.Build();

        app.Run();
    }
}