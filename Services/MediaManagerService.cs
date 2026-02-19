
using OneSound.Handler;
using WindowsMediaController;

namespace OneSound.Services;

public class MediaManagerService : IHostedService
{
    private readonly MediaManager mediaManager;
    
    public MediaManagerService()
    {
        RegisteredApp.Register("SpotifyAB.SpotifyMusic_zpdnekdrzrea0!Spotify");
        RegisteredApp.Register("Chrome");
        mediaManager = new();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        mediaManager.OnAnySessionOpened += MediaEventHandler.OnSessionOpened;
        mediaManager.OnAnySessionClosed += MediaEventHandler.OnSessionClosed;
        mediaManager.OnAnyPlaybackStateChanged += MediaEventHandler.OnPlaybackStateChanged;

        mediaManager.Start();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        mediaManager.Dispose();

        return Task.CompletedTask;
    }
}