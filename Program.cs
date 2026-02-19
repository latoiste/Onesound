using WindowsMediaController;
using OneSound.Handler;
using OneSound.Utils;

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
    public static void Main()
    {
        // Maybe later add a method that launches on start to populate registeredAumids before initializing MediaManager
        RegisteredApp.Register("SpotifyAB.SpotifyMusic_zpdnekdrzrea0!Spotify");
        RegisteredApp.Register("Chrome");

        var mediaManager = new MediaManager();

        mediaManager.OnAnySessionOpened += MediaEventHandler.OnSessionOpened;
        mediaManager.OnAnySessionClosed += MediaEventHandler.OnSessionClosed;
        mediaManager.OnAnyPlaybackStateChanged += MediaEventHandler.OnPlaybackStateChanged;

        mediaManager.Start();

        Console.ReadLine();
        Console.ResetColor();
        
        mediaManager.Dispose();
    }
}