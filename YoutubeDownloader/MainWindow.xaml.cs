using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

    string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
    public string SaveFolder
    {
        get => saveFolder;
        set
        {
            saveFolder = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveFolder)));
        }
    }

    bool isStopped = false;

    public MainWindow()
    {
        InitializeComponent();
        Items = new ObservableCollection<VideoToDownloadItem>();
        log = new ObservableCollection<string>();
        Loaded += (o, e) =>
        {
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

        SetDisableStatus(false);
        await Task.Delay(TimeSpan.FromSeconds(1));

        int total = Items.Count;
        int current = 0;
        pbIndicator.Maximum = total;

        try
        {
            foreach (var video in Items)
            {
                await LoadVideo(video);

                pbIndicator.Value = ++current;

                await PauseWait();
            }
            pbIndicator.Value = 0;
        }
        catch
        {

        }
        finally
        {
            WriteLog("Конец", "Информация");
            SetDisableStatus(true);
        }     
    }

    private async Task PauseWait()
    {
        while (isStopped)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }

    private async Task LoadVideo(VideoToDownloadItem? item)
    {
        string source = item?.Uri ?? String.Empty;
        string fileName = item?.FileName ?? string.Empty;
        string localPath = Path.Combine(SaveFolder, fileName);

        try
        {
            item.LoadStatus = LoadStatus.InProgress;
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            var loader = new YoutubeToMp3Converter();
            await loader.SaveMP3(source, localPath);
            WriteLog(fileName + " загружен", "ОК");
            item.LoadStatus = LoadStatus.Success;
        }
        catch (Exception ex)
        {
            WriteLog(ex.Message, "Ошибка");
            item.LoadStatus = LoadStatus.HasErrors;
        }
    }

    private void WriteLog(string message, string type)
    {
        LogItems.Insert(0, $"[{DateTime.Now}] - ({type}) - {message}"); 
    }

    private void btnPaseFromClipboard_Click(object sender, RoutedEventArgs e)
    {
        var lines = Clipboard.GetText()
            .Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(ar => ar.Split('\t'))
            .Select(line => new VideoToDownloadItem()
            {
                Uri = line[0],
                FileName = line[1]
            })
            .ToList();

        lines.ForEach(l => Items.Add(l));

        MessageBox.Show($"Вставлено {lines.Count} строк", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

    }

    private void btnStopContinue_Click(object sender, RoutedEventArgs e)
    {
        WriteLog(isStopped ? "Возобновлено" : "Пауза", "Информация");
        isStopped = !isStopped;        
    }
    
    private void SetDisableStatus(bool status)
    {
        btnAddVideo.IsEnabled = status;
        btnConvertToMp3.IsEnabled = status;
        btnPaseFromClipboard.IsEnabled = status;
        itemsControl.IsEnabled = status;
    }
}
