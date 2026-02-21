using WindowsMediaController;

namespace OneSound.Utils;

public static class SessionManager
{
    private static readonly HashSet<string> registeredAumids = new();
    private static readonly Dictionary<string, MediaManager.MediaSession> activeRegisteredApps = new(); 
    public static bool Register(string aumid) => registeredAumids.Add(aumid);
    public static bool IsRegistered(string aumid) => registeredAumids.Contains(aumid);
    public static bool RemoveActiveApp(string aumid) => activeRegisteredApps.Remove(aumid);
    public static void AddActiveApp(string aumid, MediaManager.MediaSession session) => activeRegisteredApps[aumid] = session;
    public static List<MediaManager.MediaSession> GetActiveRegisteredSessions() => activeRegisteredApps.Values.ToList();
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
    public static bool RemoveRegisteredAumid(string aumid) => registeredAumids.Remove(aumid);
    public static List<string> GetAvailableAumids(MediaManager mediaManager) => mediaManager.CurrentMediaSessions.Keys.ToList();
    
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
    public static void TryAutoResumeLastSession()
    {
        var lastSession = GetSessionFromId(LastPlayingSessionId);
        var controlSession = lastSession?.ControlSession;
        _ = controlSession?.TryPlayAsync();   
    }
}