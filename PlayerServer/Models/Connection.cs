using System;
using System.Net;
using System.Net.Sockets;
using PlayerServer.Services;

namespace PlayerServer.Models;

public class Connection(ServerService.Protocol protocol, IPEndPoint endPoint, IDisposable protocolClient)
{
    public ServerService.Protocol Protocol { get; set; } = protocol;
    public IPEndPoint EndPoint { get; set; } = endPoint;
    public IDisposable ProtocolClient { get; set; } = protocolClient;


    public override string ToString()
    {
        return Protocol switch
        {
            ServerService.Protocol.Tcp when ProtocolClient is TcpClient tcpClient =>
                $"Protocol={Protocol}, " +
                $"EndPoint={EndPoint}" +
                $"{(tcpClient.Connected ? ", Connected" : ", Not Connected")}",
            ServerService.Protocol.Udp when ProtocolClient is UdpClient udpClient =>
                $"Protocol={Protocol}, " +
                $"EndPoint={EndPoint}" +
                $"{udpClient}",
            _ => "Unknown Protocol"
        };
    }
}