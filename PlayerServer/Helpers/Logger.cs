using System;

namespace PlayerServer.Helpers;

public static class Logger
{
    private static Action<string>? _logAction;

    public static void Initialize(Action<string>? logAction)
    {
        _logAction = logAction;
    }

    public static void LogMessage(string message)
    {
        Console.WriteLine(message);
        _logAction?.Invoke(message);
    }

    public static void ClearLog()
    {
        _logAction?.Invoke(string.Empty);
    }
}