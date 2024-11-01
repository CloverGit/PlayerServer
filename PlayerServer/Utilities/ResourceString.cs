using Avalonia;

namespace PlayerServer.Utilities;

public static class ResourceString
{
    public static string Get(string key)
    {
        return Application.Current != null && Application.Current.Resources.TryGetResource(key, null, out var resource)
            ? resource?.ToString() ?? "Unknown"
            : "Unknown";
    }
}