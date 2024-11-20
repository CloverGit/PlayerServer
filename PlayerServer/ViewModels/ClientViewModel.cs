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

    public ObservableCollection<ClientEntryModel> ClientEntries { get; set; } = [];

    public static event Action<List<Client>>? ClientListUpdated;

    private void UpdateClients(List<Client> clients)
    {
        ClientEntries.Clear();

        foreach (var client in clients)
            ClientEntries.Add(new ClientEntryModel
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