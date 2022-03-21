using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YoutubeDownloader.Infrastructure.Commands;

namespace YoutubeDownloader.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    YoutubeToMp3Converter loader;

    ObservableCollection<YoutubeDownloadItem> items;
    public ObservableCollection<YoutubeDownloadItem> Items
    {
        get => items;
        set => Set(ref items, value);
    }

    ObservableCollection<string> logItems;
    public ObservableCollection<string> LogItems
    {
        get => logItems;
        set => Set(ref logItems, value);
    }

    YoutubeDownloadItem activeItem;
    public YoutubeDownloadItem ActiveItem
    {
        get => activeItem;
        set => Set(ref activeItem, value);
    }

    string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
    public string SaveFolder
    {
        get => saveFolder;
        set => Set(ref saveFolder, value);
    }

    bool? inProgress = null;
    public bool? InProgress
    {
        get => inProgress;
        set => Set(ref inProgress, value);
    }

    int maximumProgressValue = 1;
    public int MaximumProgressValue
    {
        get => maximumProgressValue;
        set => Set(ref maximumProgressValue, value);
    }

    int currentProgressValue = 0;
    public int CurrentProgressValue
    {
        get => currentProgressValue;
        set => Set(ref currentProgressValue, value);
    }

    public ICommand AddItemCommand { get; }
    public ICommand StartCommand { get; }
    public ICommand TogglePauseStatusCommand { get; }
    public ICommand PasteFromClipboardCommand { get; }

    public MainWindowViewModel()
    {
        Items = new ObservableCollection<YoutubeDownloadItem>();
        logItems = new ObservableCollection<string>();
        loader = new YoutubeToMp3Converter();

        loader.DownloadPercentChanged += Loader_DownloadPercentChanged;
        loader.ItemStatusChanged += (o, e) =>
        {
            string shortName = Path.GetFileName(e.LocalName);

            if (e.Status == LoadStatus.Success)
            {
                WriteLog($"[{shortName}] загружен", "OK");
            } else if (e.Status == LoadStatus.HasErrors)
            {
                WriteLog($"[{shortName}] {e.Message}", "Ошибка");
            } else
            {
                WriteLog($"[{shortName}] статус изменен: {e.Status.ToString()}", "Информация");
            }

            Items.FirstOrDefault(i => i.Uri == e.Uri).LoadStatus = e.Status;
        };

        AddItemCommand = new LambdaCommand((e) => AddItem(), e => InProgress.HasValue ? !InProgress.Value : true);
        StartCommand = new LambdaCommand(Start, CanStart);
        PasteFromClipboardCommand = new LambdaCommand(PasteFromClipboard, e => (InProgress.HasValue ? !InProgress.Value : false));
        TogglePauseStatusCommand = new LambdaCommand((e) => {
            InProgress = !InProgress.Value;
            WriteLog((InProgress.Value ? "Пауза" : "Продолжено"), "Информация"); 
        }, e => InProgress.HasValue && Items.Count > 0);      
    }

    private bool CanStart(object arg)
    {
        return (InProgress.HasValue ? !InProgress.Value : true) && Items.Count > 0 && Items.Any(i => i.IsValidUri);
    }
    private async void Start(object sender)
    {
        WriteLog("Начало", "Информация");
        InProgress = true;
        MaximumProgressValue = Items.Count;
        CurrentProgressValue = 0;

        foreach (var item in Items)
        {
            ActiveItem = item;
            string localPath = Path.Combine(SaveFolder, item.FileName);
          
            await loader.DownloadAndConvert(item.Uri, localPath);

            CurrentProgressValue++;
            await PauseWait();
        }

        WriteLog("Конец", "Информация");
        InProgress = null;
        CurrentProgressValue = MaximumProgressValue;
        CommandManager.InvalidateRequerySuggested();
    }
    private void Loader_DownloadPercentChanged(object? sender, int e)
    {
        if (ActiveItem != null)
        {
            ActiveItem.ProgressPercentage = e;
        }
    }
    private async Task PauseWait()
    {
        while (InProgress.HasValue && !InProgress.Value)
        {
            await Task.Delay(TimeSpan.FromSeconds(1.5));
        }
    }
    private void WriteLog(string message, string type)
    {
        LogItems.Insert(0, $"[{DateTime.Now}] ({type}) - {message}");
    }
    public void AddItem(string uri = null, string fileName = null)
    {
        var item = new YoutubeDownloadItem()
        {
            FileName = fileName,
            Uri = uri
        };
        item.OnRemoveClicked += () => Items.Remove(item);
        Items.Add(item);
    }
    private void PasteFromClipboard(object sender)
    {
        var lines = Clipboard.GetText()
            .Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(ar => ar.Split('\t'))
            .Select(line => new
            {
                Uri = line[0],
                FileName = line[1]
            })
            .ToList();

        lines.ForEach(l => AddItem(l.Uri, l.FileName));

        MessageBox.Show($"Вставлено {lines.Count} строк", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
