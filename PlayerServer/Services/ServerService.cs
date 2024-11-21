using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using PlayerServer.Helpers;
using PlayerServer.Models;

namespace PlayerServer.Services;

public class ServerService
{
    public enum Protocol
    {
        TCP,
        UDP
    }

    private static readonly ClientManagerService ClientManagerService = new();
    private static readonly MessageService MessageService = new(ClientManagerService);

    private readonly HashSet<Protocol> _protocols = [];
    private TcpListener? _tcpListener;
    private UdpClient? _udpClient;

    public bool Start(Protocol protocol, int port)
    {
        _protocols.Add(protocol);
        try
        {
            switch (protocol)
            {
                case Protocol.TCP:
                    _tcpListener = new TcpListener(IPAddress.Any, port);
                    _tcpListener.Start();
                    Task.Run(ListenForTcpConnections);
                    Logger.LogMessage($"{protocol} 服务已启动，侦听端口: {port}");
                    return true;
                case Protocol.UDP:
                    _udpClient = new UdpClient(port);
                    Task.Run(ListenForUdpData);
                    Logger.LogMessage($"{protocol} 服务已启动，侦听端口: {port}");
                    return true;
                default:
                    return false;
            }
        }
        catch (Exception e)
        {
            Logger.LogMessage($"{protocol} 服务启动失败:");
            Logger.LogMessage(e.Message);
            return false;
        }
    }

    public void Stop()
    {
        foreach (var protocol in _protocols)
        {
            switch (protocol)
            {
                case Protocol.TCP:
                {
                    _tcpListener?.Stop();
                    foreach (var client in ClientManagerService.GetAllClients())
                        if (client.ProtocolClient is TcpClient tcpClient)
                            tcpClient.Close();
                    Logger.LogMessage($"{protocol} 服务已停止");
                    break;
                }
                case Protocol.UDP:
                    _udpClient?.Close();
                    Logger.LogMessage($"{protocol} 服务已停止");
                    break;
                default:
                    Logger.LogMessage("未知的协议类型");
                    break;
            }

            _protocols.Remove(protocol);
            ClientManagerService.RemoveAllClients();
        }
    }

    private async Task ListenForTcpConnections()
    {
        try
        {
            while (true)
            {
                if (_tcpListener == null) continue;
                var tcpClient = await _tcpListener.AcceptTcpClientAsync();

                if (tcpClient.Client.RemoteEndPoint is not IPEndPoint remoteEndPoint)
                    throw new InvalidOperationException("RemoteEndPoint is not a valid IPEndPoint");
                var client = new Client(new Connection(Protocol.TCP, remoteEndPoint, tcpClient));
                _ = Task.Run(() => ReadTcpData(client));
                Logger.LogMessage($"新增 TCP 客户端连接: {client.EndPoint.Address}");
            }
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
        {
            Logger.LogMessage("TCP 监听器已关闭");
        }
        catch (Exception e)
        {
            Logger.LogMessage(e.Message);
        }
    }

    private static async Task ReadTcpData(Client client)
    {
        if (client.ProtocolClient is TcpClient tcpClient)
        {
            var stream = tcpClient.GetStream();
            var buffer = new byte[1024];

            try
            {
                while (true)
                {
                    var bytesRead = await stream.ReadAsync(buffer);
                    if (bytesRead == 0)
                    {
                        // 客户端断开连接，移除客户端
                        ClientManagerService.RemoveClient(client);
                        Logger.LogMessage($"客户端断开连接: {client.EndPoint.Address}");
                        break;
                    }

                    var data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Logger.LogMessage($"接收 (TCP): {data}");
                    MessageService.ParseMessage(data, client);
                }
            }
            catch (Exception e)
            {
                Logger.LogMessage(e.Message);
                ClientManagerService.RemoveClient(client);
                Logger.LogMessage($"客户端异常断开: {client.EndPoint.Address}");
            }
            finally
            {
                stream.Close();
                tcpClient.Close();
            }
        }
    }


    private async Task ListenForUdpData()
    {
        try
        {
            // var endpoint = new IPEndPoint(IPAddress.Any, _port);
            while (true)
            {
                if (_udpClient == null) continue;
                var result = await _udpClient.ReceiveAsync();
                var data = Encoding.UTF8.GetString(result.Buffer);
                Logger.LogMessage($"接收 (UDP): {data}");
                var connection = new Connection(Protocol.UDP, result.RemoteEndPoint, _udpClient);
                var client = new Client(connection);
                MessageService.ParseMessage(data, client);
            }
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
        {
            Logger.LogMessage("UDP 监听器已关闭");
        }
        catch (Exception e)
        {
            Logger.LogMessage(e.Message);
        }
    }
}