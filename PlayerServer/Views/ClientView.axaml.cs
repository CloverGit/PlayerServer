using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
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
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        // 剪贴板不可用
        if (clipboard is null) return;

        if (sender is not Button { DataContext: ClientViewModel client } button) return;
        if (button.CommandParameter is not string propertyName) return;
        var propertyInfo = client.GetType().GetProperty(propertyName);
        var value = propertyInfo?.GetValue(client) as string ?? string.Empty;

        var dataObject = new DataObject();
        dataObject.Set(DataFormats.Text, value);
        await clipboard.SetDataObjectAsync(dataObject);
    }
}