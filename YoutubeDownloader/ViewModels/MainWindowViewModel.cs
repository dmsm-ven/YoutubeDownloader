using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YoutubeDownloader.Models;
using YoutubeDownloader.Services;

namespace YoutubeDownloader.ViewModels;
public partial class MainWindowViewModel : ObservableRecipient
{
    [ObservableProperty]
    private ObservableCollection<YoutubeDownloadItem> items = new();

    [ObservableProperty]
    private ObservableCollection<string> logItems;

    [ObservableProperty]
    private YoutubeDownloadItem activeItem;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PasteFromClipboardCommand))]
    private bool isKeyboardBufferContainsYoutubeUrl = false;

    [ObservableProperty]
    private string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

    [NotifyCanExecuteChangedFor(nameof(StartCommand))]
    [NotifyCanExecuteChangedFor(nameof(AddItemCommand))]
    [NotifyCanExecuteChangedFor(nameof(PasteFromClipboardCommand))]
    [NotifyCanExecuteChangedFor(nameof(ClearAllCommand))]
    [NotifyCanExecuteChangedFor(nameof(StopCommand))]
    [ObservableProperty]
    private bool? inProgress = null;

    [ObservableProperty]
    private int maximumProgressValue = 1;

    [ObservableProperty]
    private int currentProgressValue = 0;

    private CancellationTokenSource cts;
    private readonly YoutubeTitleResolver titleResolver;
    private readonly YoutubeDownloadManager youtubeDownloadManager;

    private bool CanStart => Items.Count > 0 && Items.Any(i => i.IsValidUri) && InProgress != true;
    private bool CanPasteFromCliboard => InProgress != true && IsKeyboardBufferContainsYoutubeUrl;
    private bool CanClearAll => InProgress != true && Items.Count > 0;
    private bool CanAddItem => InProgress ?? true;
    private bool CanStop => Items.Count > 0 && InProgress == true;

    public MainWindowViewModel(YoutubeTitleResolver titleResolver, YoutubeDownloadManager youtubeDownloadManager)
    {
        this.Items.CollectionChanged += (o, e) =>
        {
            OnPropertyChanged(nameof(CanStart));
            OnPropertyChanged(nameof(CanPasteFromCliboard));
            OnPropertyChanged(nameof(CanClearAll));
            OnPropertyChanged(nameof(CanStop));
        };

        WeakReferenceMessenger.Default.Register<YoutubeDownloadItemDeletedMessage>(this, (o, i) => Receive(i));
        WeakReferenceMessenger.Default.Register<YoutubeDownloadItemUriChangedMessage>(this, (o, i) => Receive(i));
        this.titleResolver = titleResolver;
        this.youtubeDownloadManager = youtubeDownloadManager;
    }

    [RelayCommand]
    private void Loaded()
    {
        ApplicationCommands.Paste.CanExecuteChanged += async (o, e) =>
        {
            await App.Current.Dispatcher.InvokeAsync(() => IsKeyboardBufferContainsYoutubeUrl = Clipboard.GetText().Contains("https://www.youtube.com"));
        };

        //loader.ItemStatusChanged += (o, e) =>
        /*{
            string shortName = Path.GetFileName(e.LocalName);

            if (e.Status == LoadStatus.Success)
            {
                WriteLog($"[{shortName}] загружен", "OK");
            }
            else if (e.Status == LoadStatus.HasErrors)
            {
                WriteLog($"[{shortName}] {e.Message}", "Ошибка");
            }
            else
            {
                WriteLog($"[{shortName}] статус изменен: {e.Status.ToString()}", "Информация");
            }

            Items.FirstOrDefault(i => i.Uri == e.Uri).LoadStatus = e.Status;
        };

        */
    }

    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task Start()
    {
        if (InProgress.HasValue && !InProgress.Value)
        {
            WriteLog("Продолжено", "Информация");
            InProgress = true;
        }
        else
        {
            LogItems.Clear();
            WriteLog("Начало", "Информация");
            InProgress = true;
            MaximumProgressValue = Items.Count;
            CurrentProgressValue = 0;

            cts = new CancellationTokenSource();
            Progress<int> progress = new(v => CurrentProgressValue = v);

            await youtubeDownloadManager.DownloadAndConvertAll(Items, SaveFolder, progress, cts.Token);

            WriteLog("Конец", "Информация");
            InProgress = null;
            CurrentProgressValue = MaximumProgressValue;
        }
        CommandManager.InvalidateRequerySuggested();
    }

    [RelayCommand(CanExecute = nameof(CanAddItem))]
    private void AddItem(string? uri = null)
    {
        var item = new YoutubeDownloadItem()
        {
            Uri = uri
        };
        Items.Add(item);
    }

    [RelayCommand(CanExecute = nameof(CanPasteFromCliboard))]
    private void PasteFromClipboard()
    {
        Clipboard.GetText()
            .Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
            .ToList()
            .ForEach(AddItem);
    }

    [RelayCommand(CanExecute = nameof(CanClearAll))]
    private void ClearAll()
    {
        Items.Clear();
    }

    [RelayCommand(CanExecute = nameof(CanStop))]
    private void Stop()
    {
        InProgress = false;
        WriteLog("Пауза", "Информация");
    }

    public void Receive(YoutubeDownloadItemDeletedMessage message)
    {
        this.Items.Remove(message.Value);
    }

    public async void Receive(YoutubeDownloadItemUriChangedMessage message)
    {
        message.Value.FileName = await titleResolver.GetTitleByUri(message.Value.Uri);
    }

    private void WriteLog(string message, string type)
    {
        LogItems.Insert(0, $"[{DateTime.Now}] ({type}) - {message}");
    }

}
