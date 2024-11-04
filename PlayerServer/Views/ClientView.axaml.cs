using Avalonia.Controls;
using Avalonia.Interactivity;
using PlayerServer.Utilities;
using PlayerServer.ViewModels;

namespace PlayerServer.Views;

public partial class ClientView : UserControl
{
    public ClientView()
    {
        InitializeComponent();
        // DataContext = new ClientViewModel();
    }

    private async void CopyButton_Click(object? sender, RoutedEventArgs args)
    {
        if (sender is not Button { DataContext: ClientViewModel client } button) return;
        if (button.CommandParameter is not string propertyName) return;
        var propertyInfo = client.GetType().GetProperty(propertyName);
        var value = propertyInfo?.GetValue(client) as string ?? string.Empty;

        await Clipboard.SetText(value);
    }
}