using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VideoLibrary;

namespace YoutubeDownloader;

public class YoutubeToMp3Converter
{
    public YoutubeToMp3Converter()
    {

    }

    public async Task SaveMP3(string uri, string localpath, IProgress<int> progress)
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
            await DownloadSource(vid, tmpFile, progress);
            await ConvertToMp3(tmpFile, localpath);

            progress?.Report(100);
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

    private async Task DownloadSource(YouTubeVideo videoInfo, string tmpFile, IProgress<int> progress)
    {
        var buffer = new Memory<byte>(new byte[4096]);
        long readedBytes = 0;
        long? totalBytes = videoInfo.ContentLength;

        using(var fs = new FileStream(tmpFile, FileMode.CreateNew))
        {
            using (var remoteStream = await videoInfo.StreamAsync())
            {
                while (readedBytes < totalBytes)
                {
                    var readed = await remoteStream.ReadAsync(buffer);
                    readedBytes += readed;
                    await fs.WriteAsync(buffer);

                    int progressPercent = (int)(((double)readedBytes / totalBytes) * 100);
                    progress?.Report(progressPercent);
                }
                progress?.Report(100);
            }
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


}
