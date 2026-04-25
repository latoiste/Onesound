using System.Text.Json;
using System.Text.Json.Serialization;

namespace OneSound.Settings;

public class UserSettings
{
    private readonly string filePath;
    private readonly JsonSerializerOptions options = new();
    private readonly RootSettings rootSettings;

    public UserSettings()
    {
        filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user_settings.json");
        
        options.WriteIndented = true;
        
        rootSettings = GetDeserializedJson();
    }

    public void AddRegisteredAumid(string aumid)
    {
        rootSettings.RegisteredAumids.Add(aumid);

        File.WriteAllText(filePath, JsonSerializer.Serialize(rootSettings, JsonContext.Default.RootSettings));
    }

    public void RemoveRegisteredAumid(string aumid)
    {
        rootSettings.RegisteredAumids.Remove(aumid);
        
        File.WriteAllText(filePath, JsonSerializer.Serialize(rootSettings, JsonContext.Default.RootSettings));
    }

    public List<string> ReadRegisteredAumid() => rootSettings.RegisteredAumids;

    private RootSettings GetDeserializedJson()
    {
        string jsonString = File.ReadAllText(filePath);
        RootSettings? s = JsonSerializer.Deserialize(jsonString, JsonContext.Default.RootSettings);

        if (s == null) throw new Exception($"Failed to parse {Path.GetFullPath(filePath)}");

        return s!;
    }
}

[JsonSerializable(typeof(RootSettings))]
public partial class JsonContext : JsonSerializerContext { }