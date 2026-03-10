using System.Text.Json;

namespace OneSound.Saves;

public class Settings
{
    private readonly string filePath = @"user_settings.json";
    private readonly JsonSerializerOptions options = new();
    private readonly RootSettings rootSettings;

    public Settings()
    {
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

    private RootSettings GetDeserializedJson()
    {
        string jsonString = File.ReadAllText(filePath);
        RootSettings? s = JsonSerializer.Deserialize<RootSettings>(jsonString);

        if (s == null) throw new Exception($"Failed to parse {Path.GetFullPath(filePath)}");

        return s!;
    }
}