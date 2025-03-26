using System;
using System.Collections.ObjectModel;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.ViewModels.DesignTime;

public class DesignSource
{
    public ObservableCollection<YoutubeDownloadItem> DesignYoutubeDownloadItems { get; } = new();

    public DesignSource()
    {
        for (int i = 0; i < 10; i++)
        {
            DesignYoutubeDownloadItems.Add(new() { Uri = "https://youtube.com/watch?v=" + Guid.NewGuid() });
        }
    }
}
