using Windows.Media.Control;
using WindowsMediaController;

namespace OneSound.Handler;

class MediaEventHandler
{
    static readonly object _writeLock = new object();

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

        if (RegisteredApp.RemoveActiveApp(aumid))
        {
            Console.WriteLine($"{aumid} is no longer active");
        }
    }

    public static void OnPlaybackStateChanged(MediaManager.MediaSession session, GlobalSystemMediaTransportControlsSessionPlaybackInfo playbackInfo)
    {
        if (playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Opened
            || playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Closed)
        return;

        WriteLineColor($"--Playback status of {session.Id} is now {playbackInfo.PlaybackStatus}", ConsoleColor.Yellow);
        
        if (playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused)
        {
            // TODO: Play last non-forced paused media (check if it still exists)
            return;
        }

        foreach (var activeApp in RegisteredApp.GetActiveSessions())
        {
            var appPlaybackStatus = activeApp.ControlSession.GetPlaybackInfo().PlaybackStatus;
            if (activeApp.Id != session.Id)
            {
                WriteLineColor($"Pausing {activeApp.Id}...", ConsoleColor.White);
                Task.Run(() => TryPauseSessionAsync(activeApp));
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
        lock (_writeLock)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + toprint);
        }
    }
}