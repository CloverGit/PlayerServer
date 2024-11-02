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
    }

    private void LogMessageScrollToEnd(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { Text: not null } textBox)
            textBox.CaretIndex = textBox.Text.Length;
    }
}