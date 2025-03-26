using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Windows;
using YoutubeDownloader.Services;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly Mutex mutex;
    private readonly IHost AppHost;

    public App()
    {
        mutex = new Mutex(true, "YoutubeDownloaderAppInstance");
        if (!mutex.WaitOne())
        {
            Current.Shutdown();
            return;
        }

        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddHttpClient();
                services.AddSingleton<IYoutubeDownloader, SharpGrabberYoutubeDownloader>();
                services.AddSingleton<YoutubeDownloadManager>();
                services.AddSingleton<YoutubeTitleResolver>();
                services.AddSingleton<MainWindowViewModel>();
            })
            .Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var mw = new MainWindow();
        mw.DataContext = AppHost.Services.GetService<MainWindowViewModel>();
        mw.Show();
    }
}
