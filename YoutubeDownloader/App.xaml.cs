using System.Threading;
using System.Windows;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    Mutex mutex;
    protected override void OnStartup(StartupEventArgs e)
    {
        MutexCheck();

        var mw = new MainWindow();
        mw.DataContext = new MainWindowViewModel();
        mw.Show();
    }

    private void MutexCheck()
    {
        mutex = new Mutex(true, "YoutubeDownloaderAppInstance");
        if (!mutex.WaitOne())
        {
            Current.Shutdown();
            return;
        }
    }
}
