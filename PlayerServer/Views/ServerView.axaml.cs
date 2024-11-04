using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
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
        viewModel.RequestCloseFlyout += CloseFlyoutMenu;
    }

    private void OnLogMessageTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { Text: not null } textBox)
            textBox.CaretIndex = textBox.Text.Length;
    }

    private void OnLogMessageContextRequested(object sender, ContextRequestedEventArgs e)
    {
        ShowFlyoutMenu(true);
        e.Handled = true;
    }

    private void ShowFlyoutMenu(bool isTransient)
    {
        if (Resources["LogMessageFlyout"] is not CommandBarFlyout flyout) return;
        flyout.ShowMode = isTransient ? FlyoutShowMode.Transient : FlyoutShowMode.Standard;

        var textBox = this.FindControl<TextBox>("LogMessage");
        if (textBox != null) flyout.ShowAt(textBox);
    }

    private void CloseFlyoutMenu()
    {
        if (Resources["LogMessageFlyout"] is CommandBarFlyout flyout)
            flyout.Hide();
    }
}