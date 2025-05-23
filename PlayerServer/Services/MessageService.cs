using System.Net.Sockets;
using System.Text;
using PlayerServer.Helpers;
using PlayerServer.Models;

namespace PlayerServer.Services;

public class MessageService(ClientManagerService clientManager)
{
    private static void SendMessage(string message, Client recipientClient)
    {
        var data = Encoding.UTF8.GetBytes(message);

        switch (recipientClient.Protocol)
        {
            case ServerService.Protocol.TCP when recipientClient.ProtocolClient is TcpClient tcpClient:
                var stream = tcpClient.GetStream();
                stream.Write(data, 0, data.Length);
                break;

            case ServerService.Protocol.UDP when recipientClient.ProtocolClient is UdpClient udpClient:
                udpClient.Send(data, data.Length, recipientClient.EndPoint);
                break;

            default:
                Logger.LogMessage("Unknown Protocol");
                break;
        }
    }

    public void BroadcastMessage(string message)
    {
        foreach (var client in clientManager.GetAllClients())
            SendMessage(message, client);
    }

    private void ForwardMessage(Message message, string recipientId)
    {
        var recipientClient = clientManager.GetClientById(recipientId);
        if (recipientClient != null)
        {
            Logger.LogMessage($"Forwarding message to {recipientId}");
            SendMessage(message.ToForwardJson(), recipientClient);
        }
        else
        {
            Logger.LogMessage($"Cannot find client with ID: {recipientId}");
        }
    }

    public void ParseMessage(string data, Client senderClient)
    {
        var message = Message.FromJson(data);
        if (message?.Type == null)
        {
            Logger.LogMessage("Invalid message format");
            return;
        }

        var logMessage = message.Type switch
        {
            "register" => HandleClientRegistration(message, senderClient),
            "unregister" => "Unregistered",
            _ => HandleMessageForwarding(message)
        };
        Logger.LogMessage(logMessage);
    }

    private string HandleClientRegistration(Message message, Client senderClient)
    {
        if (message.FromId == string.Empty)
            return "Invalid registration message";

        senderClient.Id = message.FromId;
        clientManager.RegisterClient(senderClient);
        return $"Client registered with ID: {senderClient.Id}";
    }

    private string HandleMessageForwarding(Message message)
    {
        if (message.ToIds.Count == 0)
            return "Invalid forward message";

        foreach (var toId in message.ToIds)
            ForwardMessage(message, toId);

        return "All messages have been forwarded";
    }
}