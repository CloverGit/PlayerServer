using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PlayerServer.Models;

public class Setting
{
    public bool AutoStartServer { get; set; } = true;
    public int DefaultPort { get; set; }
    public string DefaultProtocol { get; set; }
    public List<string> SupportedProtocols { get; set; } = [];
    public string DefaultLanguage { get; set; }
    public List<string> SupportedLanguages { get; set; } = [];
    public string DefaultTheme { get; set; }
    public List<string> SupportedThemes { get; set; } = [];
    public List<string> DefaultListeningProtocols { get; set; } = [];

    // 序列化
    public void SaveToFile(string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var jsonString = JsonSerializer.Serialize(this, options);
        File.WriteAllText(filePath, jsonString);
    }

    // 反序列化
    public static Setting LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException("Settings file not found");

        var jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<Setting>(jsonString);
    }
}