using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using PlayerServer.Helpers;

namespace PlayerServer.Models;

public class Message(List<string> ids)
{
    private Message() : this([])
    {
    }

    public string? Type { get; private set; }
    public string? Extra { get; private set; }
    public List<string> Ids { get; private set; } = ids;

    public static Message? FromJson(string jsonData)
    {
        try
        {
            var messageData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonData);

            if (messageData == null || !messageData.TryGetValue("type", out var typeElement))
                return null;

            var message = new Message
            {
                Type = typeElement.GetString(),
                Extra = messageData.TryGetValue("extra", out var value) ? value.GetString() : null,
                Ids = messageData.TryGetValue("id", out var idElement) && idElement.ValueKind == JsonValueKind.Array
                    // 使用 Where 来过滤 null 值将产生一个警告
                    // ? idElement.EnumerateArray().Select(id => id.GetString()).Where(id => id != null).ToList()
                    ? idElement.EnumerateArray().Select(id => id.GetString() ?? string.Empty).ToList()
                    : []
            };

            return message;
        }
        catch (JsonException ex)
        {
            Logger.LogMessage("Error parsing JSON: {ex.Message}");
            return null;
        }
    }

    public string ToForwardJson()
    {
        return JsonSerializer.Serialize(new
        {
            type = Type,
            extra = Extra
        });
    }
}