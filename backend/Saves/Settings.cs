using System.Text.Json;

namespace OneSound.Saves;

public class Settings
{
    private readonly string filePath;
    private readonly JsonSerializerOptions options = new();
    private readonly RootSettings rootSettings;

    public Settings()
    {
        filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user_settings.json");
        
        options.WriteIndented = true;
        
        rootSettings = GetDeserializedJson();
    }

    public void AddRegisteredAumid(string aumid)
    {
        rootSettings.RegisteredAumids.Add(aumid);

        File.WriteAllText(filePath, JsonSerializer.Serialize(rootSettings, options));
    }

    public void RemoveRegisteredAumid(string aumid)
    {
        rootSettings.RegisteredAumids.Remove(aumid);
        
        File.WriteAllText(filePath, JsonSerializer.Serialize(rootSettings, options));
    }

    public List<string> ReadRegisteredAumid() => rootSettings.RegisteredAumids;

    private RootSettings GetDeserializedJson()
    {
        string jsonString = File.ReadAllText(filePath);
        RootSettings? s = JsonSerializer.Deserialize<RootSettings>(jsonString);

        if (s == null) throw new Exception($"Failed to parse {Path.GetFullPath(filePath)}");

        return s!;
    }
}