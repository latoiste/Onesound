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

    private static void WriteLineColor(object toprint, ConsoleColor color = ConsoleColor.White)
    {
        lock (_writeLock)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + toprint);
        }
    }
}