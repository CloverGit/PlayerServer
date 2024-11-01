using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using PlayerServer.Models;

namespace PlayerServer.ViewModels;

public class ClientViewModel : ViewModelBase
{
    public ClientViewModel()
    {
        ClientListUpdated += UpdateClients;
    }

    public string? Header { get; set; }
    public string? Name { get; set; }
    public string? Ip { get; set; }
    public string? Port { get; set; }
    public string? ConnectionTime { get; set; }

    public ObservableCollection<ClientViewModel> Clients { get; set; } = [];

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
}