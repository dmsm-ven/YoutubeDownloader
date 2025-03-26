using System.Threading.Tasks;

namespace YoutubeDownloader.Services;

public interface IYoutubeDownloader
{
    Task DownloadYoutubeVideo(string videoCode);
    Task DownloadYoutubeAudio(string videoCode);
}

public class SharpGrabberYoutubeDownloader : IYoutubeDownloader
{

    public SharpGrabberYoutubeDownloader()
    {

    }
    public Task DownloadYoutubeAudio(string videoCode)
    {
        throw new System.NotImplementedException();
    }

    public Task DownloadYoutubeVideo(string videoCode)
    {
        throw new System.NotImplementedException();
    }
}

