using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Services;

public class YoutubeDownloadManager
{
    private readonly IYoutubeDownloader youtubeDownloader;

    public YoutubeDownloadManager(IYoutubeDownloader youtubeDownloader)
    {
        this.youtubeDownloader = youtubeDownloader;
    }

    public async Task DownloadAndConvertAll(IEnumerable<YoutubeDownloadItem> items,
        string saveFolder,
        IProgress<int>? progress = null,
        CancellationToken? token = null)
    {

        int current = 0;
        int total = items.Count();

        foreach (var item in items)
        {
            await DownloadAndConvert(item, saveFolder, token);

            progress?.Report(++current);
            token?.ThrowIfCancellationRequested();
        }
    }

    private async Task DownloadAndConvert(YoutubeDownloadItem item, string saveFolder, CancellationToken? token)
    {
        string code = HttpUtility.ParseQueryString(new Uri(item.Uri).Query).Get("v");
        await youtubeDownloader.DownloadYoutubeAudio(code);
    }
}

