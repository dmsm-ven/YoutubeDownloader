using System.ComponentModel;
using System.Runtime.CompilerServices;

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
            RaisePropertyChanged();
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
        }
    }

    private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
