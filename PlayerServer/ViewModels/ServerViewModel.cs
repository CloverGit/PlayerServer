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
    // 默认选中 TCP UDP
    private readonly HashSet<ServerService.Protocol> _selectedProtocols =
        [ServerService.Protocol.TCP, ServerService.Protocol.UDP];

    private readonly ServerService _serverService = new();

    private string _logMessages = string.Empty;

    // 1346 为默认测试端口
    private int _serverPort = 1346;
    private bool _serverRunningState;

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
        ResourceString.Get(ServerRunningState ? "ServerIsRunningString" : "ServerIsNotRunningString");

    public string ServerIpAddress { get; } = GetLocalIpAddress();

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
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return ip.ToString();
        return "undefined";
    }

    private void StartServer()
    {
        var port = _serverPort;
        if (port is <= 0 or > 65535)
        {
            Logger.LogMessage(ResourceString.Get("InvalidPortString"));
            return;
        }

        if (_selectedProtocols.Count == 0)
        {
            Logger.LogMessage(ResourceString.Get("NoProtocolSelectedString"));
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