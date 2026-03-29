using OneSound.Settings;
using OneSound.Media.Manager;
namespace OneSound.Media.Session;

public class SessionManager
{
    private readonly MediaManager mediaManager;
    private readonly LastMediaTimer timer;
    private readonly UserSettings settings;
    private readonly HashSet<string> registeredAumids;
    private readonly Dictionary<string, MediaSession> activeRegisteredApps; 

    public string LastPlayingSessionId { 
        get;
        set
        {
            field = value;
            if (value != "") timer.UpdateLastMediaTimer();
        }
    }

    public SessionManager(MediaManager mediaManager)
    {
        this.mediaManager = mediaManager;

        timer = new();
        timer.OnTimerTimeout += OnTimerTimeout;

        settings = new();
        registeredAumids = new();
        activeRegisteredApps = new();
        LastPlayingSessionId = "";

        foreach (var aumid in settings.ReadRegisteredAumid())
        {
            registeredAumids.Add(aumid);
        }
    }

    public List<string> GetRegisteredAumids() => registeredAumids.ToList();
    
    public List<string> GetAvailableAumids() => mediaManager.CurrentMediaSessions.Keys.ToList();

    public bool Register(string aumid) {
        settings.AddRegisteredAumid(aumid);
        return registeredAumids.Add(aumid);
    }

    public bool RemoveRegisteredAumid(string aumid) {
        settings.RemoveRegisteredAumid(aumid);
        return registeredAumids.Remove(aumid);
    }

    public bool IsRegistered(string aumid) => registeredAumids.Contains(aumid);

    public bool RemoveActiveApp(string aumid) => activeRegisteredApps.Remove(aumid);

    public void AddActiveApp(string aumid, MediaSession session) => activeRegisteredApps[aumid] = session;

    public List<MediaSession> GetActiveRegisteredSessions() => activeRegisteredApps.Values.ToList();

    public MediaSession? GetSessionFromId(string aumid)
    {
        try
        {
            var session = mediaManager.CurrentMediaSessions[aumid];
            return session;
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    public void TryAutoResumeLastSession()
    {
        var lastSession = GetSessionFromId(LastPlayingSessionId);
        _ = lastSession?.PlayAsync();   
    }
    
    private void OnTimerTimeout()
    {
        LastPlayingSessionId = "";
    }
}
