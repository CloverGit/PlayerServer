using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using PlayerServer.Helpers;
using PlayerServer.Services;
using PlayerServer.Utilities;

namespace PlayerServer.ViewModels;

public class ServerViewModel : ViewModelBase
{
    private readonly HashSet<ServerService.Protocol> _selectedProtocols;

    private readonly ServerService _serverService;

    private string _logMessages;

    private int _serverPort;
    private bool _serverRunningState;

    public ServerViewModel()
    {
        // 默认选中 TCP UDP
        _selectedProtocols =
        [
            ServerService.Protocol.TCP,
            ServerService.Protocol.UDP
        ];
        _serverService = new ServerService();
        _logMessages = string.Empty;
        // 1346 为默认测试端口
        _serverPort = 1346;
        ServerRunningState = false;
        ServerIpAddress = GetLocalIpAddress();
    }

    public string LogMessages
    {
        get => _logMessages;
        private set => SetProperty(ref _logMessages, value);
    }

    public string ServerPort
    {
        get => _serverPort.ToString();
        set
        {
            if (!int.TryParse(value, out var port)) return;
            SetProperty(ref _serverPort, port);
        }
    }

    public bool ServerRunningState
    {
        get => _serverRunningState;
        set
        {
            SetProperty(ref _serverRunningState, value);
            OnPropertyChanged(nameof(ServerRunningStateText));

            if (_serverRunningState) StartServer();
            else StopServer();
        }
    }

    public string ServerRunningStateText =>
        Resource.Get<string>(ServerRunningState ? "ServerIsRunningString" : "ServerIsNotRunningString");

    public string ServerIpAddress { get; }

    public bool IsTcpChecked
    {
        get => _selectedProtocols.Contains(ServerService.Protocol.TCP);
        set
        {
            if (value) _selectedProtocols.Add(ServerService.Protocol.TCP);
            else _selectedProtocols.Remove(ServerService.Protocol.TCP);
            OnPropertyChanged();
        }
    }

    public bool IsUdpChecked
    {
        get => _selectedProtocols.Contains(ServerService.Protocol.UDP);
        set
        {
            if (value) _selectedProtocols.Add(ServerService.Protocol.UDP);
            else _selectedProtocols.Remove(ServerService.Protocol.UDP);
            OnPropertyChanged();
        }
    }

    public event Action? RequestCloseFlyout;

    private static string GetLocalIpAddress()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
        }
        catch (Exception ex)
        {
            Logger.LogMessage($"Cannot get local IP address: {ex.Message}");
        }

        return "undefined";
    }

    private void StartServer()
    {
        var port = _serverPort;
        if (port is <= 0 or > 65535)
        {
            Logger.LogMessage(Resource.Get<string>("InvalidPortString"));
            return;
        }

        if (_selectedProtocols.Count == 0)
        {
            Logger.LogMessage(Resource.Get<string>("NoProtocolSelectedString"));
            return;
        }

        foreach (var protocol in _selectedProtocols)
            _serverService.Start(protocol, port);
    }

    private void StopServer()
    {
        _serverService.Stop();
    }

    public void LogMessageClearCommand()
    {
        AppendLogMessage(string.Empty);
        RequestCloseFlyout?.Invoke();
    }

    public void LogMessageSaveCommand()
    {
        // todo
        RequestCloseFlyout?.Invoke();
    }

    public void LogMessageCopyCommand()
    {
        _ = Clipboard.SetText(LogMessages);
        RequestCloseFlyout?.Invoke();
    }

    public void AppendLogMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            LogMessages = string.Empty;
        else
            LogMessages += message + Environment.NewLine;
    }
}