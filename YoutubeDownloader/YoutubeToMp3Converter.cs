using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VideoLibrary;

namespace YoutubeDownloader;

public class YoutubeToMp3Converter
{
    public YoutubeToMp3Converter()
    {

    }

    public async Task SaveMP3(string uri, string localpath)
    {
        var source = Path.GetDirectoryName(localpath);
        if (!Path.GetExtension(localpath).Equals(".mp3"))
        {
            localpath += ".mp3";
        }
        var youtube = YouTube.Default;
        var vid = youtube.GetVideo(uri);

        string tmpFile = Path.GetTempFileName();
        try
        {
            await Task.Run(() => File.WriteAllBytes(tmpFile, vid.GetBytes()));

            var inputFile = new MediaFile { Filename = tmpFile };
            var outputFile = new MediaFile { Filename = localpath };

            using (var engine = new Engine())
            {
                await Task.Run(() =>
                {
                    engine.GetMetadata(inputFile);
                    engine.Convert(inputFile, outputFile);
                });
            }
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
}
