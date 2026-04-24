using OneSound.Settings;
using OneSound.Media.Manager;
namespace OneSound.Media.Session;

public class SessionManager : IDisposable
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

        mediaManager.OnSessionOpened += OnSessionOpened;
        mediaManager.OnSessionClosed += OnSessionClosed;

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
    
    public List<string> GetAvailableAumids() => mediaManager.GetAllIds();

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

    public void TryAutoResumeLastSession()
    {
        var lastSession = mediaManager.GetSessionFromId(LastPlayingSessionId);
        _ = lastSession?.PlayAsync();   
    }
    
    private void OnTimerTimeout()
    {
        LastPlayingSessionId = "";
    }

    private void OnSessionOpened(MediaSession session) => AddActiveApp(session.Id, session);
    private void OnSessionClosed(MediaSession session) => RemoveActiveApp(session.Id);

    public void Dispose()
    {
        mediaManager.OnSessionOpened -= OnSessionOpened;
        mediaManager.OnSessionClosed -= OnSessionClosed;
    }
}
