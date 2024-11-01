using System.Net;
using System.Net.Sockets;
using Avalonia.Controls;
using PlayerServer.Helpers;
using PlayerServer.ViewModels;

namespace PlayerServer.Views;

public partial class ServerView : UserControl
{
    public ServerView()
    {
        InitializeComponent();

        var viewModel = new ServerViewModel();
        DataContext = viewModel;
        Logger.Initialize(viewModel.AppendLogMessage);
        var ipAddress = GetLocalIpAddress();
        IpAddress.Text = ipAddress;
    }

    private static string GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return ip.ToString();
        return "undefined";
    }
}