using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using PlayerServer.Services;
using PlayerServer.Utilities;

namespace PlayerServer.ViewModels;

public class ServerViewModel : ViewModelBase
{
    // 默认选中 TCP UDP
    private readonly HashSet<ServerService.Protocol> _selectedProtocols =
        [ServerService.Protocol.Tcp, ServerService.Protocol.Udp];

    private readonly ServerService _serverService = new();

    private string _logMessages = string.Empty;

    // 1346 为默认测试端口
    private int _serverPort = 1346;
    private bool _serverRunningState;

    public string LogMessages
    {
        get => _logMessages;
        private set
        {
            if (_logMessages == value) return;
            _logMessages = value;
            OnPropertyChanged();
        }
    }

    public string ServerPort
    {
        get => _serverPort.ToString();
        set
        {
            if (!int.TryParse(value, out var port)) return;
            if (_serverPort == port) return;
            _serverPort = port;
            OnPropertyChanged();
        }
    }

    public bool ServerRunningState
    {
        get => _serverRunningState;
        set
        {
            if (_serverRunningState == value) return;
            _serverRunningState = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ServerRunningStateText));

            if (_serverRunningState) StartServer();
            else StopServer();
        }
    }

    public string ServerRunningStateText =>
        ResourceString.Get(ServerRunningState ? "ServerIsRunningString" : "ServerIsNotRunningString");

    public string ServerIpAddress { get; } = GetLocalIpAddress();

    public bool IsTcpChecked
    {
        get => _selectedProtocols.Contains(ServerService.Protocol.Tcp);
        set
        {
            if (value) _selectedProtocols.Add(ServerService.Protocol.Tcp);
            else _selectedProtocols.Remove(ServerService.Protocol.Tcp);
            OnPropertyChanged();
        }
    }

    public bool IsUdpChecked
    {
        get => _selectedProtocols.Contains(ServerService.Protocol.Udp);
        set
        {
            if (value) _selectedProtocols.Add(ServerService.Protocol.Udp);
            else _selectedProtocols.Remove(ServerService.Protocol.Udp);
            OnPropertyChanged();
        }
    }

    private static string GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return ip.ToString();
        return "undefined";
    }

    private void StartServer()
    {
        var port = _serverPort;
        foreach (var protocol in _selectedProtocols)
            _serverService.Start(protocol, port);
    }

    private void StopServer()
    {
        _serverService.Stop();
    }

    public void AppendLogMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            LogMessages = string.Empty;
        else
            LogMessages += message + Environment.NewLine;
    }
}