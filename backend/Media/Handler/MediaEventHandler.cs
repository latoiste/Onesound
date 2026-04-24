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

    public void OnSessionOpened(MediaSession session)
    {
        WriteLineColor("-- New Source: " + session.Id, ConsoleColor.Green);
        string aumid = session.Id;

        if (!sessionManager.IsRegistered(aumid)) return;
        
        Console.WriteLine($"{aumid} is active!!!");

        if (session.GetPlaybackStatus() == SessionStatus.Playing)
        {
            PauseOtherSessions(session);
        }
    }

    public void OnSessionClosed(MediaSession session)
    {
        WriteLineColor("-- Removed Source: " + session.Id, ConsoleColor.Red);
        string aumid = session.Id;

        Console.WriteLine($"{aumid} is no longer active");
        
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
                PauseOtherSessions(session);
                break;
            case SessionStatus.Paused:
                if (sessionManager.LastPlayingSessionId != session.Id) sessionManager.TryAutoResumeLastSession();
                break;
        }
    }

    private void PauseOtherSessions(MediaSession currentSession)
    {
        foreach (var activeApp in sessionManager.GetActiveRegisteredSessions())
        {
            if (activeApp.Id != currentSession.Id && activeApp.GetPlaybackStatus() == SessionStatus.Playing)
            {
                activeApp.PauseAsync();

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