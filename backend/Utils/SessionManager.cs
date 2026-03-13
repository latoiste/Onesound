using OneSound.Saves;
using WindowsMediaController;

namespace OneSound.Utils;

public class SessionManager
{
    private readonly MediaManager mediaManager;
    private readonly LastMediaTimer timer;
    private readonly Settings settings;

    public SessionManager(MediaManager mediaManager)
    {
        this.mediaManager = mediaManager;

        timer = new();
        timer.OnTimerTimeout += OnTimerTimeout;

        settings = new();

        foreach (var aumid in settings.ReadRegisteredAumid())
        {
            registeredAumids.Add(aumid);
        }
    }

    private readonly HashSet<string> registeredAumids = new();
    private readonly Dictionary<string, MediaManager.MediaSession> activeRegisteredApps = new(); 

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
    public void AddActiveApp(string aumid, MediaManager.MediaSession session) => activeRegisteredApps[aumid] = session;
    public List<MediaManager.MediaSession> GetActiveRegisteredSessions() => activeRegisteredApps.Values.ToList();
    public MediaManager.MediaSession? GetSessionFromId(string aumid)
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
    
    private string lastPlayingSessionId = "";

    public string LastPlayingSessionId { 
        get => lastPlayingSessionId;
        set
        {
            lastPlayingSessionId = value;
            if (value != "")
            {
                Console.WriteLine($"Last playing session is now {lastPlayingSessionId}");
                timer.UpdateLastMediaTimer();
            }
        }
    }

    private void OnTimerTimeout()
    {
        LastPlayingSessionId = "";
    }

    public void TryAutoResumeLastSession()
    {
        var lastSession = GetSessionFromId(LastPlayingSessionId);
        var controlSession = lastSession?.ControlSession;
        _ = controlSession?.TryPlayAsync();   
    }
}