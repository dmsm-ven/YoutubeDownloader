using System.ComponentModel;

namespace YoutubeDownloader;

public class VideoToDownloadItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    LoadStatus loadStatus;
    public LoadStatus LoadStatus
    {
        get => loadStatus;
        set
        {
            loadStatus = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadStatus)));
        }
    }

    string uri;
    public string Uri
    {
        get => uri;
        set
        {
            uri = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Uri)));
        }
    }

    string fileName;
    public string FileName
    {
        get => fileName;
        set
        {
            fileName = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileName)));
        }
    }
}
