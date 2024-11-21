using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using PlayerServer;

internal sealed class Program
{
    private static Task Main(string[] args)
    {
        return BuildAvaloniaApp()
            .WithFont_SourceHanSansCN()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>();
    }
}