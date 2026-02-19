using WindowsMediaController;

namespace OneSound.Utils;

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