using OneSound.Utils;
using Windows.Media.Control;
using WindowsMediaController;

namespace OneSound.Handler;

public class MediaEventHandler
{
    private static readonly object writeLock = new object();

    private readonly SessionManager sessionManager;

    public MediaEventHandler(SessionManager sessionManager)
    {
        this.sessionManager = sessionManager;
    }

    public void OnSessionOpened(MediaManager.MediaSession session)
    {
        WriteLineColor("-- New Source: " + session.Id, ConsoleColor.Green);
        string aumid = session.Id;

        if (sessionManager.IsRegistered(aumid))
        {
            sessionManager.AddActiveApp(aumid, session);
            Console.WriteLine($"{aumid} is active!!!");
        }
    }

    public void OnSessionClosed(MediaManager.MediaSession session)
    {
        WriteLineColor("-- Removed Source: " + session.Id, ConsoleColor.Red);
        string aumid = session.Id;

        if (sessionManager.RemoveActiveApp(aumid)) Console.WriteLine($"{aumid} is no longer active");
        
        if (aumid == sessionManager.LastPlayingSessionId) sessionManager.LastPlayingSessionId = "";
    }

    public void OnPlaybackStateChanged(MediaManager.MediaSession session, GlobalSystemMediaTransportControlsSessionPlaybackInfo playbackInfo)
    {
        if (!sessionManager.IsRegistered(session.Id)) return;

        WriteLineColor($"--Playback status of {session.Id} is now {playbackInfo.PlaybackStatus}", ConsoleColor.Yellow);
        
        switch (playbackInfo.PlaybackStatus)
        {
            case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing:
                PauseOtherSessions(session);
                break;
            case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused:
                if (sessionManager.LastPlayingSessionId != session.Id) sessionManager.TryAutoResumeLastSession();
                break;
        }
    }

    private void PauseOtherSessions(MediaManager.MediaSession currentSession)
    {
        foreach (var activeApp in sessionManager.GetActiveRegisteredSessions())
        {
            if (activeApp.Id != currentSession.Id && activeApp.ControlSession.GetPlaybackInfo().PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
            {
                var _ = activeApp.ControlSession.TryPauseAsync();

                sessionManager.LastPlayingSessionId = activeApp.Id;
            } 
        }
    }

    private static void WriteLineColor(object toprint, ConsoleColor color = ConsoleColor.White)
    {
        lock (writeLock)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + toprint);
        }
    }
}