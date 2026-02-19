using WindowsMediaController;
using OneSound.Utils;
using OneSound.Services;

namespace OneSound;

public static class RegisteredApp
{
    private static readonly HashSet<string> registeredAumids = new();
    private static readonly Dictionary<string, MediaManager.MediaSession> activeRegisteredApps = new(); 
    public static void Register(string aumid) => registeredAumids.Add(aumid);
    public static bool IsRegistered(string aumid) => registeredAumids.Contains(aumid);
    public static bool RemoveActiveApp(string aumid) => activeRegisteredApps.Remove(aumid);
    public static void AddActiveApp(string aumid, MediaManager.MediaSession session) => activeRegisteredApps.Add(aumid, session);
    public static List<MediaManager.MediaSession> GetActiveSessions() => activeRegisteredApps.Values.ToList();
    public static MediaManager.MediaSession? GetSessionFromId(string aumid)
    {
        try
        {
            var session = activeRegisteredApps[aumid];
            return session;
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }
}

public class SessionManager
{
    private static string lastPlayingSessionId = "";
    public static string LastPlayingSessionId { 
        get => lastPlayingSessionId;
        set
        {
            lastPlayingSessionId = value;
            if (value != "")
            {
                Console.WriteLine($"Last playing session is now {lastPlayingSessionId}");
                LastMediaTimer.UpdateLastMediaTimer();
            }
        } 
    }
}

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