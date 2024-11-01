using System;
using System.Collections.ObjectModel;
using PlayerServer.Services;
using PlayerServer.Utilities;

namespace PlayerServer.ViewModels;

public class ServerViewModel : ViewModelBase
{
    private readonly ServerService _serverService = new();

    private string _logMessages;

    private string _selectedProtocol;

    private int _serverPort;
    private bool _serverRunningState;

    public ServerViewModel()
    {
        SupportProtocols = ["TCP", "UDP"];
        // 1346 为默认测试端口
        _serverPort = 1346;
        _serverRunningState = false;
        // 默认选中 TCP
        _selectedProtocol = SupportProtocols[0];
        _logMessages = string.Empty;
    }

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

    public string SelectedProtocol
    {
        get => _selectedProtocol;
        set
        {
            if (_selectedProtocol == value) return;
            _selectedProtocol = value;
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

    public ObservableCollection<string> SupportProtocols { get; }

    private void StartServer()
    {
        var port = _serverPort;
        var protocol = _selectedProtocol == "TCP" ? ServerService.Protocol.Tcp : ServerService.Protocol.Udp;

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