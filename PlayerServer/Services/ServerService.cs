using System;
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
        Tcp,
        Udp
    }

    private static readonly ClientManagerService ClientManagerService = new();
    private static readonly MessageService MessageService = new(ClientManagerService);

    private Protocol? _protocol;
    private TcpListener TcpListener;
    private UdpClient UdpClient;

    public bool Start(Protocol protocol, int port)
    {
        _protocol = protocol;

        switch (protocol)
        {
            case Protocol.Tcp:
                TcpListener = new TcpListener(IPAddress.Any, port);
                TcpListener.Start();
                Task.Run(ListenForTcpConnections);
                Logger.LogMessage($"TCP 服务端已启动，侦听端口: {port}");
                return true;
            case Protocol.Udp:
                UdpClient = new UdpClient(port);
                Task.Run(ListenForUdpData);
                Logger.LogMessage($"UDP 服务端已启动，侦听端口: {port}");
                return true;
            default:
                return false;
        }
    }

    public void Stop()
    {
        switch (_protocol)
        {
            case Protocol.Tcp:
            {
                TcpListener.Stop();
                foreach (var client in ClientManagerService.GetAllClients())
                    if (client.ProtocolClient is TcpClient tcpClient)
                        tcpClient.Close();
                break;
            }
            case Protocol.Udp:
                UdpClient.Close();
                break;
            default:
                Logger.LogMessage("未知的协议类型");
                break;
        }

        ClientManagerService.RemoveAllClients();
        Logger.ClearLog();
        Logger.LogMessage("服务端已停止");
    }

    private async Task ListenForTcpConnections()
    {
        try
        {
            while (true)
            {
                var tcpClient = await TcpListener.AcceptTcpClientAsync();

                if (tcpClient.Client.RemoteEndPoint is not IPEndPoint remoteEndPoint)
                    throw new InvalidOperationException("RemoteEndPoint is not a valid IPEndPoint");
                var client = new Client(new Connection(Protocol.Tcp, remoteEndPoint, tcpClient));
                _ = Task.Run(() => ReadTcpData(client));
                Logger.LogMessage($"新增 TCP 客户端连接: {client.EndPoint.Address}");
            }
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
        {
            Logger.LogMessage("监听器已关闭");
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
        }
    }


    private async Task ListenForUdpData()
    {
        try
        {
            // var endpoint = new IPEndPoint(IPAddress.Any, _port);
            while (true)
            {
                var result = await UdpClient.ReceiveAsync();
                var data = Encoding.UTF8.GetString(result.Buffer);
                Logger.LogMessage($"接收 (UDP): {data}");
                var connection = new Connection(Protocol.Udp, result.RemoteEndPoint, UdpClient);
                var client = new Client(connection);
                MessageService.ParseMessage(data, client);
            }
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
        {
            Logger.LogMessage("监听器已关闭");
        }
        catch (Exception e)
        {
            Logger.LogMessage(e.Message);
        }
    }
}