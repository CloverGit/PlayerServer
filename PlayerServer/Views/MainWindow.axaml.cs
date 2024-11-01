using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using PlayerServer.ViewModels;

namespace PlayerServer.Views;

public partial class MainWindow : AppWindow
{
    private readonly ClientViewModel _clientViewModel;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();

        // 提前实例化 ClientViewModel 以避免在未加载时View时添加数据后无法显示
        // 这个实现不太好, 改成通过依赖注入实现单例比较好
        _clientViewModel = new ClientViewModel();
    }

    private void MainNavigationSelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        var tag = (string)((NavigationViewItem)e.SelectedItem).Tag!;
        var type = tag switch
        {
            "ServerView" => typeof(ServerView),
            "ClientView" => typeof(ClientView),
            "SettingView" => typeof(SettingView),
            _ => typeof(ServerView)
        };
        ContentFrame.Navigate(type);

        if (type == typeof(ClientView))
            (ContentFrame.Content as ClientView).DataContext = _clientViewModel;
    }
}