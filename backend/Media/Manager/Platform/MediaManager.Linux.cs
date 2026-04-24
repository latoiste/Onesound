using System.Threading.Channels;
using OneSound.Media.Session;
using Tmds.DBus.Protocol;

namespace OneSound.Media.Manager;

public class MediaManagerLinux : MediaManager
{
    private string serviceNamePrefix = "org.mpris.MediaPlayer2.";
    private readonly string dBusSessionAddress;
    private readonly DBusConnection connection;
    private IDisposable? nameOwnerChangedSubscription;
    private Dictionary<string, IDisposable?> propertiesChangedSubscriptions = new();
    private readonly Channel<NameOwnerChangedSignature> channel = Channel.CreateUnbounded<NameOwnerChangedSignature>();
    private bool isStarted = false;

    public MediaManagerLinux()
    {
        if (isStarted) return;

        dBusSessionAddress = DBusAddress.Session ?? throw new Exception("Environment variable DBUS_SESSION_BUS_ADDRESS not set");
        connection = new DBusConnection(dBusSessionAddress);
    }

    public override async Task StartAsync()
    {
        if (isStarted) return;

        await connection.ConnectAsync();

        await GetAllActiveSessions();

        MatchRule nameOwnerChangedRule = new()
        {
            Type = MessageType.Signal,
            Sender = "org.freedesktop.DBus",
            Interface = "org.freedesktop.DBus",
            Member = "NameOwnerChanged",
            Path = "/org/freedesktop/DBus"
        };

        nameOwnerChangedSubscription = await connection.AddMatchAsync<NameOwnerChangedSignature>(
            rule: nameOwnerChangedRule,
            reader: static (message, _) =>
            {
                Reader reader = message.GetBodyReader();

                var name = reader.ReadString();
                var oldOwner = reader.ReadString();
                var newOwner = reader.ReadString();

                NameOwnerChangedSignature signature = new(name, oldOwner, newOwner);

                return signature;
            },
            handler: (ex, signature, _, __) => {
                if (ex != null) throw ex;

                if (signature.name.StartsWith(serviceNamePrefix)) channel.Writer.TryWrite(signature);
            },
            flags: ObserverFlags.None
        );

        _ = StartChannelConsumer();
        isStarted = true;
    }

    private async Task StartChannelConsumer()
    {
        while (await channel.Reader.WaitToReadAsync())
        {
            channel.Reader.TryRead(out NameOwnerChangedSignature signature);

            await NameOwnerChangedHandlerAsync(signature);
        }
    }

    private async Task GetAllActiveSessions()
    {
        string[] services = await connection.ListServicesAsync();
        foreach (var s in services)
        {
            if (s.StartsWith(serviceNamePrefix))
            {
                MediaSessionLinux session = new MediaSessionLinux(s, connection);
                await NameAcquiredHandlerAsync(session);
            }
        }
    }

    private async Task NameOwnerChangedHandlerAsync(NameOwnerChangedSignature signature)
    {
        MediaSessionLinux session = new MediaSessionLinux(signature.name, connection);

        if (string.IsNullOrEmpty(signature.oldOwner) && !string.IsNullOrEmpty(signature.newOwner)) {
            await NameAcquiredHandlerAsync(session);
        }
        else if (!string.IsNullOrEmpty(signature.oldOwner) && string.IsNullOrEmpty(signature.newOwner)) {
            NameLostHandler(session);
        }
    }

    private async Task NameAcquiredHandlerAsync(MediaSession session)
    {
        await HookWatchPropertiesSignal(session);

        NotifySessionOpened(session);
    }

    private void NameLostHandler(MediaSession session)
    {
        string id = session.Id;

        IDisposable? subscription = propertiesChangedSubscriptions[id];
        propertiesChangedSubscriptions.Remove(id);
        subscription?.Dispose();

        NotifySessionClosed(session);
    }

    private async Task HookWatchPropertiesSignal(MediaSession session)
    {
        MatchRule propertiesChangedRule = new()
        {
            Type = MessageType.Signal,
            Sender = session.Id,
            Interface = "org.freedesktop.DBus.Properties",
            Member = "PropertiesChanged",
            Path = "/org/mpris/MediaPlayer2"
        };

        IDisposable subscription = await connection.AddMatchAsync<PropertiesChangedSignature>(
            rule: propertiesChangedRule,
            reader: static (message, _) =>
            {
                Reader reader = message.GetBodyReader();

                // s a{sv} as
                string interfaceName = reader.ReadString();
                Dictionary<string, VariantValue> changedProperties = reader.ReadDictionaryOfStringToVariantValue();
                List<string> invalidatedProperties = reader.ReadArrayOfString().ToList();

                PropertiesChangedSignature signature = new(interfaceName, changedProperties, invalidatedProperties);

                return signature;
            },
            handler: (ex, signature, _, __) => {
                if (ex != null) throw ex;
                
                PropertiesChangedHandler(signature, session);
            },
            flags: ObserverFlags.None
        );

        string id = session.Id;
        propertiesChangedSubscriptions[id] = subscription;
    }

    private void PropertiesChangedHandler(PropertiesChangedSignature signature, MediaSession session)
    {
        signature.changedProperties.TryGetValue("PlaybackStatus", out VariantValue statusString);

        if (statusString.Type == VariantValueType.Invalid) return;
        
        NotifyPlaybackStateChanged(session, statusString.ToString().ToSessionStatus());
    }

    public override void Dispose()
    {
        base.Dispose();

        nameOwnerChangedSubscription?.Dispose();

        foreach (IDisposable? s in propertiesChangedSubscriptions.Values) {
            s?.Dispose();
        }
    }
}

public readonly struct NameOwnerChangedSignature
{
    public NameOwnerChangedSignature(string name, string oldOwner, string newOwner)
    {
        this.name = name;
        this.oldOwner = oldOwner;
        this.newOwner = newOwner;
    }

    public readonly string name;
    public readonly string oldOwner;
    public readonly string newOwner;
}

public readonly struct PropertiesChangedSignature
{
    public PropertiesChangedSignature(string interfaceName, Dictionary<string, VariantValue> changedProperties, List<string> invalidatedProperties)
    {
        this.interfaceName = interfaceName;
        this.changedProperties = changedProperties;
        this.invalidatedProperties = invalidatedProperties;
    }

    public readonly string interfaceName;
    public readonly Dictionary<string, VariantValue> changedProperties;
    public readonly List<string> invalidatedProperties;
}

public class MediaSessionLinux : MediaSession
{
    private readonly DBusConnection connection;

    // make id here the service path
    public MediaSessionLinux(string id, DBusConnection connection) : base(id)
    {
        this.connection = connection;
    }

    public override SessionStatus GetPlaybackStatus()
    {
        // TODO: get status ahdjadh

        return SessionStatus.Playing;
    }

    public override async Task PauseAsync()
    {
        await connection.CallMethodAsync(createMediaPlayerMessage("Pause"));
    }

    public override async Task PlayAsync()
    {
        await connection.CallMethodAsync(createMediaPlayerMessage("Play"));
    }

    // dbus-send --session 
    // --type=method_call 
    // --dest=org.mpris.MediaPlayer2.spotify 
    // path /org/mpris/MediaPlayer2 
    // method org.mpris.MediaPlayer2.Player.Play
    private MessageBuffer createMediaPlayerMessage(string member)
    {
        using var writer = connection.GetMessageWriter();

        writer.WriteMethodCallHeader(
            destination: Id,
            path: "/org/mpris/MediaPlayer2",
            @interface: "org.mpris.MediaPlayer2.Player",
            member: member
        );

        return writer.CreateMessage();
    }

    // dbus-send --session 
    // --type=method_call 
    // --dest=org.mpris.MediaPlayer2.spotify 
    // path /org/mpris/MediaPlayer2 
    // method org.freedesktop.DBus.Properties
}