using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace PlayerServer.Utilities;

public static class Clipboard
{
    public static async Task SetText(string text)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.Clipboard is not { } provider)
            throw new InvalidOperationException("Missing Clipboard instance.");

        await provider.SetTextAsync(text);
    }

    public static async Task<string> GetText()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.Clipboard is not { } provider)
            throw new InvalidOperationException("Missing Clipboard instance.");

        return await provider.GetTextAsync() ?? string.Empty;
    }
}