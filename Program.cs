using WindowsMediaController;
using OneSound.Handler;
namespace OneSound;

public class RegisteredApp
{
    public static readonly HashSet<string> registeredAumids = new();
    private static readonly Dictionary<string, MediaManager.MediaSession> activeRegisteredApps = new(); 
    public static void Register(string aumid) => registeredAumids.Add(aumid);
    public static bool IsRegistered(string aumid) => registeredAumids.Contains(aumid);
    public static bool RemoveActiveApp(string aumid) => activeRegisteredApps.Remove(aumid);
    public static void AddActiveApp(string aumid, MediaManager.MediaSession session) => activeRegisteredApps.Add(aumid, session);
}

public class Program
{
    public static void Main()
    {
        // Maybe later add a method that launches on start to populate registeredAumids before initializing MediaManager
        RegisteredApp.Register("SpotifyAB.SpotifyMusic_zpdnekdrzrea0!Spotify");

        var mediaManager = new MediaManager();

        mediaManager.OnAnySessionOpened += MediaEventHandler.OnSessionOpened;
        mediaManager.OnAnySessionClosed += MediaEventHandler.OnSessionClosed;

        mediaManager.Start();
        

        Console.ReadLine();
        mediaManager.Dispose();
    }
}