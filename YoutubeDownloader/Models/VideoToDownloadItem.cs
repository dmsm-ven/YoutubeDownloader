using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Input;
using YoutubeDownloader.Infrastructure.Commands;

namespace YoutubeDownloader;

public class YoutubeDownloadItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action OnRemoveClicked;
    public event Action OnTimerByTimerClicked;

    LoadStatus loadStatus;
    public LoadStatus LoadStatus
    {
        get => loadStatus;
        set
        {
            loadStatus = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(ProgressStatus));
        }
    }

    string uri;
    public string Uri
    {
        get => uri;
        set
        {
            uri = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsValidUri));
        }
    }

    string fileName;
    public string FileName
    {
        get => fileName;
        set
        {
            fileName = value;
            RaisePropertyChanged();
        }
    }

    int progressPercentage;
    public int ProgressPercentage
    {
        get => progressPercentage;
        set
        {
            if (progressPercentage != value)
            {
                progressPercentage = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ProgressStatus));
            }
        }
    }
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
    private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public ICommand RemoveItemCommand { get; }
    public ICommand LoadByTimerCommand { get; }
    public YoutubeDownloadItem()
    {
        RemoveItemCommand = new LambdaCommand((e) => OnRemoveClicked?.Invoke(), e => true);
        LoadByTimerCommand = new LambdaCommand((e) => OnTimerByTimerClicked?.Invoke(), e => true);
    }


    public bool IsValidUri => !string.IsNullOrWhiteSpace(Uri) && uri.StartsWith("https://www.youtube.com");
}
