using OneSound.Media.Session;

namespace OneSound.Media.Manager;

public abstract class MediaManager
{
    public readonly Dictionary<string, MediaSession> CurrentMediaSessions = new();
    
    public abstract void Start();

    public event Action<MediaSession> OnSessionOpened;
    public event Action<MediaSession> OnSessionClosed;
    public event Action<MediaSession, SessionStatus> OnPlaybackStateChanged;

    public void AddCurrentMediaSession(string id, MediaSession session) => CurrentMediaSessions[id] = session;
    public void RemoveCurrentMediaSession(string id) => CurrentMediaSessions.Remove(id);

    protected void NotifySessionOpened(MediaSession session) => OnSessionOpened?.Invoke(session);
    protected void NotifySessionClosed(MediaSession session) => OnSessionClosed?.Invoke(session);
    protected void NotifyPlaybackStateChanged(MediaSession session, SessionStatus status) => OnPlaybackStateChanged?.Invoke(session, status);
}

public abstract class MediaSession
{
    public readonly string Id;

    public MediaSession(string id)
    {
        Id = id;
    }

    public abstract SessionStatus GetPlaybackStatus();
    public abstract Task PlayAsync();
    public abstract Task PauseAsync();
}