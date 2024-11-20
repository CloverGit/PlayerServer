using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PlayerServer.Utilities;

namespace PlayerServer.ViewModels;

public class ClientEntryModel : ViewModelBase
{
    public ClientEntryModel()
    {
        CopyCommand = new RelayCommand<object>(CopyToClipboard);
    }

    public string? Header { get; set; }
    public string? Name { get; set; }
    public string? Ip { get; set; }
    public string? Port { get; set; }
    public string? ConnectionTime { get; set; }

    public ICommand CopyCommand { get; }

    private void CopyToClipboard(object? parameter)
    {
        if (parameter is not string propertyName) return;
        // 使用反射获取绑定的属性值
        var property = GetType().GetProperty(propertyName);
        var value = property?.GetValue(this) as string;
        if (!string.IsNullOrEmpty(value))
            _ = Clipboard.SetText(value);
    }
}