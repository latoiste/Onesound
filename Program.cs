using WindowsMediaController;
namespace OneSound;

public class Program
{
    public static void Main()
    {
        var manager = new MediaManager();
        manager.Start();
        
        var mediaSesions = manager.CurrentMediaSessions;
        foreach (var session in mediaSesions.Keys)
        {
            Console.WriteLine(session);
        }
        //GOOD ENOGUH
        Console.ReadLine();

        manager.Dispose();
    }
}