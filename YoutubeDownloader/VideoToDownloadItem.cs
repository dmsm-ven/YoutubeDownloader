using System.ComponentModel;

namespace YoutubeDownloader;

public class VideoToDownloadItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    bool isLoaded;
    public bool IsLoaded
    {
        get => isLoaded;
        set
        {
            isLoaded = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoaded)));
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
