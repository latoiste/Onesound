
using OneSound.Handler;
using WindowsMediaController;

namespace OneSound.Services;

public class MediaManagerService : IHostedService
{
    private readonly MediaManager mediaManager;
    private readonly MediaEventHandler eventHandler;
    
    public MediaManagerService(MediaManager mediaManager, MediaEventHandler eventHandler)
    {
        this.mediaManager = mediaManager;
        this.eventHandler = eventHandler;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        mediaManager.OnAnySessionOpened += eventHandler.OnSessionOpened;
        mediaManager.OnAnySessionClosed += eventHandler.OnSessionClosed;
        mediaManager.OnAnyPlaybackStateChanged += eventHandler.OnPlaybackStateChanged;

        mediaManager.Start();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // no need to call mediaManager.Dispose() now because
        // mediaManager is dependency injected and DI container
        // will dispose it automatically when app shuts down
        // or else a NullReferenceException is thrown

        return Task.CompletedTask;
    }
}