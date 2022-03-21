using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YoutubeDownloader;

public class YoutubeDownloadItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

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
            progressPercentage = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(ProgressStatus));
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
}
