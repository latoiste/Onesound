using Windows.Media.Control;
using WindowsMediaController;

namespace OneSound.Handler;

public class MediaEventHandler
{
    private static readonly object writeLock = new object();

    public static void OnSessionOpened(MediaManager.MediaSession session)
    {
        WriteLineColor("-- New Source: " + session.Id, ConsoleColor.Green);
        string aumid = session.Id;

        if (RegisteredApp.IsRegistered(aumid))
        {
            RegisteredApp.AddActiveApp(aumid, session);
            Console.WriteLine($"{aumid} is active!!!");
        }
    }

    public static void OnSessionClosed(MediaManager.MediaSession session)
    {
        WriteLineColor("-- Removed Source: " + session.Id, ConsoleColor.Red);
        string aumid = session.Id;

        if (RegisteredApp.RemoveActiveApp(aumid)) Console.WriteLine($"{aumid} is no longer active");
        
        if (aumid == SessionManager.LastPlayingSessionId) SessionManager.LastPlayingSessionId = "";
    }

    public static void OnPlaybackStateChanged(MediaManager.MediaSession session, GlobalSystemMediaTransportControlsSessionPlaybackInfo playbackInfo)
    {
        if (playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Opened
            || playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Closed)
        return;

        WriteLineColor($"--Playback status of {session.Id} is now {playbackInfo.PlaybackStatus}", ConsoleColor.Yellow);
        
        switch (playbackInfo.PlaybackStatus)
        {
            case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing:
                PauseOtherSessions(session);
                break;
            case GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused:
                if (SessionManager.LastPlayingSessionId != session.Id)
                {
                    var lastSession = RegisteredApp.GetSessionFromId(SessionManager.LastPlayingSessionId);
                    var controlSession = lastSession?.ControlSession;
                    _ = controlSession?.TryPlayAsync();   
                }
                break;
        }
    }

    private static void PauseOtherSessions(MediaManager.MediaSession currentSession)
    {
        foreach (var activeApp in RegisteredApp.GetActiveSessions())
        {
            if (activeApp.Id != currentSession.Id && activeApp.ControlSession.GetPlaybackInfo().PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
            {
                WriteLineColor($"Pausing {activeApp.Id}...", ConsoleColor.White);
                var _ = TryPauseSessionAsync(activeApp);

                SessionManager.LastPlayingSessionId = activeApp.Id;
            } 
        }
    }

    private static async Task TryPauseSessionAsync(MediaManager.MediaSession session)
    {
        var controlSession = session.ControlSession;
        bool isPaused = await controlSession.TryPauseAsync();
        
        WriteLineColor(isPaused == true ? $"{session.Id} has been paused successfully" : $"{session.Id} failed to pause", ConsoleColor.Magenta);
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