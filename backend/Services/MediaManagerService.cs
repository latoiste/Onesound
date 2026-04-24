
using OneSound.Media.Handler;
using OneSound.Media.Manager;

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

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        mediaManager.OnSessionOpened += eventHandler.OnSessionOpened;
        mediaManager.OnSessionClosed += eventHandler.OnSessionClosed;
        mediaManager.OnPlaybackStateChanged += eventHandler.OnPlaybackStateChanged;

        await mediaManager.StartAsync();
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