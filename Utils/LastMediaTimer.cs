namespace OneSound.Utils;

public class LastMediaTimer
{
    private static CancellationTokenSource? cts;

    public static void UpdateLastMediaTimer()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = new();

        var _ =  StartTimerAsync(cts.Token);
    }

    private static async Task StartTimerAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(TimeSpan.FromMinutes(3), token);
            Console.WriteLine($"Last playing session is no longer {SessionManager.LastPlayingSessionId}");
            SessionManager.LastPlayingSessionId = "";
        } 
        catch (OperationCanceledException)
        {
            Console.WriteLine("Token was cancelled");
        }
    }
}