using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace YoutubeDownloader;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    ObservableCollection<VideoToDownloadItem> items;
    public ObservableCollection<VideoToDownloadItem> Items
    {
        get => items;
        set
        {
            items = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
        }
    }
    ObservableCollection<string> log;
    public ObservableCollection<string> LogItems
    {
        get => log;
        set
        {
            log = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogItems)));
        }
    }

    public MainWindow()
    {
        InitializeComponent();
        Items = new ObservableCollection<VideoToDownloadItem>();
        log = new ObservableCollection<string>();
        Loaded += (o, e) =>
        {
            AddItem(null, null);
            DataContext = this;
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void AddItem(object sender, RoutedEventArgs e)
    {
        Items.Add(new VideoToDownloadItem());
    }

    private async void btnConvertToMp3_Click(object sender, RoutedEventArgs e)
    {
        bool isCancel = false;
        foreach(var item in Items)
        {
            if (string.IsNullOrWhiteSpace(item.Uri) || !item.Uri.StartsWith("https://www.youtube.com") || string.IsNullOrWhiteSpace(item.FileName))
            {
                isCancel = true;
            }
        }
        if (isCancel)
        {
            MessageBox.Show("Не все данные заполнены", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        WriteLog("Начало", "Информация");

        int total = Items.Count;
        int current = 0;
        pbIndicator.Maximum = total;

        foreach (var video in Items)
        {
            await LoadVideo(video);

            pbIndicator.Value = ++current;
        }
        pbIndicator.Value = 0;

        WriteLog("Конец", "Информация");
        
    }

    private async Task LoadVideo(VideoToDownloadItem? video)
    {
        string source = video?.Uri ?? String.Empty;
        string fileName = video?.FileName ?? string.Empty;
        string localPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), fileName);

        try
        {
            var loader = new YoutubeToMp3Converter();
            await loader.SaveMP3(source, localPath);
            WriteLog(fileName + " загружен", "ОК");
        }
        catch (Exception ex)
        {
            WriteLog(ex.Message, "Ошибка");
        }
    }

    private void WriteLog(string message, string type)
    {
        LogItems.Add($"[{DateTime.Now}] - ({type}) - {message}"); 
    }
}
