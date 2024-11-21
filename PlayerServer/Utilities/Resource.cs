using Avalonia;
using Avalonia.Controls;

namespace PlayerServer.Utilities;

public static class Resource
{
    public static T Get<T>(string key)
    {
        try
        {
            Application.Current!.TryFindResource(key, out var value);
            return (T)value!;
        }
        catch
        {
            return typeof(T) == typeof(string) ? (T)(object)"??" : default!;
        }
    }
}