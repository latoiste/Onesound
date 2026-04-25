#if WINDOWS
using OneSound.Media.Session;
using Windows.Media.Control;

namespace OneSound.Media.Manager;

public class MediaManagerWindows : MediaManager
{
    private WindowsMediaController.MediaManager mediaManager;
    private bool isStarted = false;

    public MediaManagerWindows()
    {
        mediaManager = new WindowsMediaController.MediaManager();

        mediaManager.OnAnySessionOpened += SessionOpenedTranslator;
        mediaManager.OnAnySessionClosed += SessionClosedTranslator;
        mediaManager.OnAnyPlaybackStateChanged += PlaybackStateChangedTranslator;
    }

    public override async Task StartAsync() {
        if (isStarted) return;

        await mediaManager.StartAsync();
        isStarted = true;
    }

    public override void Dispose()
    {
        mediaManager.Dispose();
    }

    private void SessionOpenedTranslator(WindowsMediaController.MediaManager.MediaSession winSession) => NotifySessionOpened(new MediaSessionWindows(winSession.Id, winSession.ControlSession));

    private void SessionClosedTranslator(WindowsMediaController.MediaManager.MediaSession winSession) => NotifySessionClosed(new MediaSessionWindows(winSession.Id, winSession.ControlSession));

    private void PlaybackStateChangedTranslator(WindowsMediaController.MediaManager.MediaSession winSession, GlobalSystemMediaTransportControlsSessionPlaybackInfo winPlaybackInfo)
    {
        string id = winSession.Id;

        MediaSessionWindows session = new(id, winSession.ControlSession);
        SessionStatus status = winPlaybackInfo.PlaybackStatus.ToSessionStatus();

        NotifyPlaybackStateChanged(session, status);
    }
}

public class MediaSessionWindows : MediaSession
{
    private readonly GlobalSystemMediaTransportControlsSession controlSession;

    public MediaSessionWindows(string id, GlobalSystemMediaTransportControlsSession controlSession) : base(id)
    {
        this.controlSession = controlSession;
    }

    public override Task<SessionStatus> GetPlaybackStatusAsync()
    {
        GlobalSystemMediaTransportControlsSessionPlaybackStatus playbackStatus = controlSession.GetPlaybackInfo().PlaybackStatus;
        return Task.FromResult(playbackStatus.ToSessionStatus());
    }

    public override async Task PauseAsync() => await controlSession.TryPauseAsync();

    public override async Task PlayAsync() => await controlSession.TryPlayAsync();
}
#endif