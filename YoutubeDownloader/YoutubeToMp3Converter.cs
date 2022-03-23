using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using VideoLibrary;

namespace YoutubeDownloader;

public class YoutubeToMp3Converter
{
    public bool Paused { get; set; }

    public event EventHandler<YoutubeConverterStatusChangedArgs> ItemStatusChanged;
    public event EventHandler<int> DownloadPercentChanged;

    public async Task DownloadAndConvert(string youtubeUri, string localPath)
    {
        try
        {
            
            await SaveMP3(youtubeUri, localPath);
            ItemStatusChanged?.Invoke(this, new YoutubeConverterStatusChangedArgs(LoadStatus.Success, $"Выполнено", youtubeUri, localPath));
        }
        catch (Exception ex)
        {
            ItemStatusChanged?.Invoke(this, new YoutubeConverterStatusChangedArgs(LoadStatus.HasErrors, $"Ошибка выполнения: {ex.Message}", youtubeUri, localPath));
        }
    }
    private async Task SaveMP3(string uri, string localpath)
    {
        var source = Path.GetDirectoryName(localpath);
        if (!Path.GetExtension(localpath).Equals(".mp3"))
        {
            localpath += ".mp3";
        }
        var youtube = YouTube.Default;
        YouTubeVideo vid = await youtube.GetVideoAsync(uri);
        string tmpFile = localpath.Replace(Path.GetFileName(localpath), "_" + Path.GetFileName(localpath));

        try
        {
            await PauseWait();

            ItemStatusChanged?.Invoke(this, new YoutubeConverterStatusChangedArgs(LoadStatus.Downloading, $"Начало скачивания", uri, localpath));
            await DownloadSource(vid, tmpFile);

            ItemStatusChanged?.Invoke(this, new YoutubeConverterStatusChangedArgs(LoadStatus.Converting, $"Начало конвертации", uri, localpath));
            await ConvertToMp3(tmpFile, localpath);
        }
        catch
        {
            throw;
        }
        finally
        {
            File.Delete(tmpFile);
        }
    }
    private async Task DownloadSource(YouTubeVideo videoInfo, string tmpFile)
    {
        using(var wc = new WebClient())
        {
            wc.DownloadProgressChanged += (o, e) =>
            {
                DownloadPercentChanged?.Invoke(this, e.ProgressPercentage);
            };
            await wc.DownloadFileTaskAsync(videoInfo.Uri, tmpFile);
        }
    }
    private async Task ConvertToMp3(string tmpFile, string localpath)
    {
        var inputFile = new MediaFile { Filename = tmpFile };
        var outputFile = new MediaFile { Filename = localpath };

        await Task.Run(() =>
        {
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
                engine.Convert(inputFile, outputFile);
            }
        });
    }
    private async Task PauseWait()
    {
        while (Paused)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
