using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PlayerServer.Models;
using PlayerServer.Utilities;

namespace PlayerServer.ViewModels;

public class ClientViewModel : ViewModelBase
{
    public ClientViewModel()
    {
        ClientListUpdated += UpdateClients;
        CopyCommand = new RelayCommand<object>(CopyToClipboard);
    }

    public string? Header { get; set; }
    public string? Name { get; set; }
    public string? Ip { get; set; }
    public string? Port { get; set; }
    public string? ConnectionTime { get; set; }

    public ObservableCollection<ClientViewModel> Clients { get; set; } = [];
    public ICommand CopyCommand { get; }

    public static event Action<List<Client>>? ClientListUpdated;

    private void UpdateClients(List<Client> clients)
    {
        Clients.Clear();
        foreach (var client in clients)
            Clients.Add(new ClientViewModel
            {
                Header = client.Id,
                Name = client.Name,
                Ip = client.EndPoint.Address.ToString(),
                Port = client.EndPoint.Port.ToString(),
                ConnectionTime = client.ConnectionTime.ToString(CultureInfo.CurrentCulture)
            });
        OnPropertyChanged();
    }

    public static void OnClientListUpdated(List<Client> clients)
    {
        ClientListUpdated?.Invoke(clients);
    }

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