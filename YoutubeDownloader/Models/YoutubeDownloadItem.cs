using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader.Models;

public partial class YoutubeDownloadItem : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressStatus))]
    private LoadStatus loadStatus;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsValidUri))]
    private string uri;

    partial void OnUriChanged(string uri)
    {
        WeakReferenceMessenger.Default.Send(new YoutubeDownloadItemUriChangedMessage(this));
    }

    [ObservableProperty]
    private string fileName;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressStatus))]
    private int progressPercentage;

    public string ProgressStatus
    {
        get
        {
            if (LoadStatus == LoadStatus.Downloading)
            {
                return $"Загрузка видео: {ProgressPercentage}%";
            }
            if (LoadStatus == LoadStatus.Converting)
            {
                return $"Конвертация MP4 -> MP3";
            }
            if (LoadStatus == LoadStatus.HasErrors)
            {
                return $"Ошибка загрузки";
            }
            return string.Empty;
        }
    }

    public bool IsValidUri => !string.IsNullOrWhiteSpace(Uri) && uri.StartsWith("https://www.youtube.com");

    [RelayCommand]
    private void RemoveItem()
    {
        WeakReferenceMessenger.Default.Send(new YoutubeDownloadItemDeletedMessage(this));
    }
}
