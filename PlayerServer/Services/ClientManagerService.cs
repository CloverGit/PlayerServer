using System.Collections.Generic;
using PlayerServer.Helpers;
using PlayerServer.Models;
using PlayerServer.ViewModels;

namespace PlayerServer.Services;

public class ClientManagerService
{
    private readonly List<Client> _clients = [];

    public void RegisterClient(Client client)
    {
        var existingClient = GetClientById(client.Id);
        if (existingClient != null)
        {
            Logger.LogMessage($"客户端连接信息已存在: {existingClient}");
            Logger.LogMessage($"更新客户端信息: {client}");

            existingClient.EndPoint = client.EndPoint;
            existingClient.ProtocolClient = client.ProtocolClient;
            existingClient.Protocol = client.Protocol;
        }
        else
        {
            _clients.Add(client);
            Logger.LogMessage($"新增客户端 ID: {client.Id}");
            Logger.LogMessage($"地址: {client.EndPoint.Address}");
            Logger.LogMessage($"端口: {client.EndPoint.Port}");
            Logger.LogMessage($"协议: {client.Protocol}");
        }
        NotifyClientUpdate();
        Logger.LogMessage($"当前客户端数量: {_clients.Count}");
    }

    public void RemoveClient(Client client)
    {
        _clients.Remove(client);
        NotifyClientUpdate();
    }

    public void RemoveAllClients()
    {
        _clients.Clear();
        NotifyClientUpdate();
    }

    public Client? GetClientById(string? id)
    {
        return _clients.Find(client => client.Id == id);
    }

    public IEnumerable<Client> GetAllClients()
    {
        return _clients;
    }

    private void NotifyClientUpdate()
    {
        ClientViewModel.OnClientListUpdated(_clients);
    }
}