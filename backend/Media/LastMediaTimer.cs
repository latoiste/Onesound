namespace OneSound.Media;

public class LastMediaTimer
{
    private static CancellationTokenSource? cts;
    
    public event Action OnTimerTimeout;

    public void UpdateLastMediaTimer()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = new();

        var _ =  StartTimerAsync(cts.Token);
    }

    private async Task StartTimerAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(TimeSpan.FromMinutes(2), token);
            OnTimerTimeout?.Invoke();
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Token was cancelled");
        }
    }
}