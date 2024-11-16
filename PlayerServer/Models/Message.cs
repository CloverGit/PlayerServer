using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using PlayerServer.Helpers;

namespace PlayerServer.Models;

public class Message(string fromId, List<string> toIds)
{
    private Message() : this(string.Empty, [])
    {
    }

    public string? Type { get; private set; }
    public string? Extra { get; private set; }
    public List<string> ToIds { get; private set; } = toIds;

    public string FromId { get; set; } = fromId;

    public static Message? FromJson(string jsonData)
    {
        try
        {
            var messageData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonData);

            if (messageData == null) return null;

            var message = new Message
            {
                Type = messageData.TryGetValue("type", out var type) ? type.GetString() : null,
                Extra = messageData.TryGetValue("extra", out var extra) ? extra.GetString() : null,
                ToIds = messageData.TryGetValue("toId", out var toIds) && toIds.ValueKind == JsonValueKind.Array
                    // 使用 Where 来过滤 null 值将产生一个警告
                    // ? toIds.EnumerateArray().Select(id => id.GetString()).Where(id => id != null).ToList()
                    ? toIds.EnumerateArray().Select(id => id.GetString() ?? string.Empty).ToList()
                    : [],
                FromId = (messageData.TryGetValue("fromId", out var fromIds) ? fromIds.GetString() : null) ??
                         string.Empty
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
            toId = ToIds,
            type = Type,
            extra = Extra,
            fromId = FromId
        });
    }
}