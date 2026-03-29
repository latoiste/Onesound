using OneSound.Media.Session;
using Windows.Media.Control;

namespace OneSound.Media.Manager;

public class MediaManagerWindows : MediaManager
{
    private WindowsMediaController.MediaManager mediaManager;

    public MediaManagerWindows()
    {
        mediaManager = new WindowsMediaController.MediaManager();

        mediaManager.OnAnySessionOpened += SessionOpenedTranslator;
        mediaManager.OnAnySessionClosed += SessionClosedTranslator;
        mediaManager.OnAnyPlaybackStateChanged += PlaybackStateChangedTranslator;
    }

    public override void Start() => mediaManager.Start();

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
    private GlobalSystemMediaTransportControlsSession controlSession;

    public MediaSessionWindows(string id, GlobalSystemMediaTransportControlsSession controlSession) : base(id)
    {
        this.controlSession = controlSession;
    }

    public override SessionStatus GetPlaybackStatus()
    {
        GlobalSystemMediaTransportControlsSessionPlaybackStatus playbackInfo = controlSession.GetPlaybackInfo().PlaybackStatus;
        return playbackInfo.ToSessionStatus();
    }

    public override async Task PauseAsync() => await controlSession.TryPauseAsync();

    public override async Task PlayAsync() => await controlSession.TryPlayAsync();
}