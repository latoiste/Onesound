namespace OneSound.Utils;

public static class SessionManager
{
    private static string lastPlayingSessionId = "";
    public static string LastPlayingSessionId { 
        get => lastPlayingSessionId;
        set
        {
            lastPlayingSessionId = value;
            if (value != "")
            {
                Console.WriteLine($"Last playing session is now {lastPlayingSessionId}");
                LastMediaTimer.UpdateLastMediaTimer();
            }
        } 
    }
}