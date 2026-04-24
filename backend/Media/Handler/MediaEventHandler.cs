using OneSound.Media.Session;
using OneSound.Media.Manager;

namespace OneSound.Media.Handler;

public class MediaEventHandler
{
    private static readonly object writeLock = new object();

    private readonly SessionManager sessionManager;

    public MediaEventHandler(SessionManager sessionManager)
    {
        this.sessionManager = sessionManager;
    }

    public async Task OnSessionOpened(MediaSession session)
    {
        WriteLineColor("-- New Source: " + session.Id, ConsoleColor.Green);
        string aumid = session.Id;

        if (!sessionManager.IsRegistered(aumid)) return;
        
        SessionStatus playbackStatus = await session.GetPlaybackStatusAsync();

        if (playbackStatus == SessionStatus.Playing) // uhhh
        {
            _ = PauseOtherSessions(session);
        }
    }

    public void OnSessionClosed(MediaSession session)
    {
        WriteLineColor("-- Removed Source: " + session.Id, ConsoleColor.Red);
        string aumid = session.Id;

        if (aumid == sessionManager.LastPlayingSessionId) 
        {
            sessionManager.LastPlayingSessionId = "";
        } else
        {
            sessionManager.TryAutoResumeLastSession();
        }
    }

    public void OnPlaybackStateChanged(MediaSession session, SessionStatus status)
    {
        if (!sessionManager.IsRegistered(session.Id)) return;

        WriteLineColor($"--Playback status of {session.Id} is now {status}", ConsoleColor.Yellow);
        
        switch (status)
        {
            case SessionStatus.Playing:
                _ = PauseOtherSessions(session);
                break;
            case SessionStatus.Paused:
                if (sessionManager.LastPlayingSessionId != session.Id) sessionManager.TryAutoResumeLastSession();
                break;
        }
    }

    private async Task PauseOtherSessions(MediaSession currentSession)
    {
        foreach (var activeApp in sessionManager.GetActiveRegisteredSessions())
        {
            SessionStatus playbackStatus = await activeApp.GetPlaybackStatusAsync();

            if (activeApp.Id != currentSession.Id && playbackStatus == SessionStatus.Playing)
            {
                _ = activeApp.PauseAsync();

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