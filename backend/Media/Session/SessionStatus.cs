#if WINDOWS
using Windows.Media.Control;
#endif

namespace OneSound.Media.Session;

public enum SessionStatus
{
    Playing,
    Paused,
    Unknwon,
}

public static class SessionStatusHelper
{
    #if WINDOWS
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
    #endif

    public static SessionStatus ToSessionStatus(this string linuxStatus)
    {
        SessionStatus status;

        switch (linuxStatus)
        {
            case "Playing [String]":
                status = SessionStatus.Playing;
                break;
            case "Paused [String]":
                status = SessionStatus.Paused;
                break;
            default:
                status = SessionStatus.Unknwon;
                break;
        }
        return status;
    }
}
