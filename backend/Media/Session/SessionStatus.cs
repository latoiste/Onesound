using Windows.Media.Control;

namespace OneSound.Media.Session;

public enum SessionStatus
{
    Playing,
    Paused,
    Unknwon,
}

public static class SessionStatusHelper
{
    public static SessionStatus ToSessionStatus(this GlobalSystemMediaTransportControlsSessionPlaybackStatus winStatus)
    {
        SessionStatus status;

        switch (winStatus)
        {
            case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing:
                status = SessionStatus.Playing;
                break;
            case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused:
                status = SessionStatus.Paused;
                break;
            default:
                status = SessionStatus.Unknwon;
                break;
        }
        return status;
    }
}
