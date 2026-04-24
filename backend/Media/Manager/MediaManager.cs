using OneSound.Media.Session;

namespace OneSound.Media.Manager;

public abstract class MediaManager : IDisposable
{
    private readonly Dictionary<string, MediaSession> CurrentMediaSessions = new();
    
    public MediaManager()
    {
        OnSessionOpened += AddCurrentMediaSessions;
        OnSessionClosed += RemoveCurrentMediaSession;
    }

    public abstract Task StartAsync();
    public virtual void Dispose()
    {
        OnSessionOpened -= AddCurrentMediaSessions;
        OnSessionClosed -= RemoveCurrentMediaSession;
    }

    public MediaSession? GetSessionFromId(string aumid)
    {
        try
        {
            var session = CurrentMediaSessions[aumid];
            return session;
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    public List<string> GetAllIds() => CurrentMediaSessions.Keys.ToList();

    public event Action<MediaSession>? OnSessionOpened;
    public event Action<MediaSession>? OnSessionClosed;
    public event Action<MediaSession, SessionStatus>? OnPlaybackStateChanged;

    protected void NotifySessionOpened(MediaSession session) => OnSessionOpened?.Invoke(session);
    protected void NotifySessionClosed(MediaSession session) => OnSessionClosed?.Invoke(session);
    protected void NotifyPlaybackStateChanged(MediaSession session, SessionStatus status) => OnPlaybackStateChanged?.Invoke(session, status);
    
    private void AddCurrentMediaSessions(MediaSession session) {
        string id = session.Id;
        CurrentMediaSessions[id] = session;
    }
    
    private void RemoveCurrentMediaSession(MediaSession session)
    {
        string id = session.Id;
        CurrentMediaSessions.Remove(id);
    }
}

public abstract class MediaSession
{
    public readonly string Id;

    public MediaSession(string id)
    {
        Id = id;
    }

    public abstract Task<SessionStatus> GetPlaybackStatusAsync();
    public abstract Task PlayAsync();
    public abstract Task PauseAsync();
}