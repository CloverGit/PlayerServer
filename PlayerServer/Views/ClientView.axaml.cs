using Avalonia.Controls;

namespace PlayerServer.Views;

public partial class ClientView : UserControl
{
    public ClientView()
    {
        InitializeComponent();
        // DataContext = new ClientViewModel();
    }

    /*
     private void UpdateClientList(List<Client> clients)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ClientItemsControl.ItemsSource = clients.ConvertAll(client => new ClientViewModel
            {
                Header = client.Id,
                Name = client.Name,
                Ip = client.EndPoint.Address.ToString(),
                Port = client.EndPoint.Port.ToString(),
                ConnectionTime = client.ConnectionTime.ToString(CultureInfo.CurrentCulture)
            });
        });
    }
    */
}