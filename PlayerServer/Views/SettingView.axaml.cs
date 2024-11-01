using Avalonia.Controls;
using PlayerServer.ViewModels;

namespace PlayerServer.Views;

public partial class SettingView : UserControl
{
    public SettingView()
    {
        InitializeComponent();
        DataContext = new SettingViewModel();
    }

    // 启动时自动开启服务器
    // 默认协议
    // 默认端口
}