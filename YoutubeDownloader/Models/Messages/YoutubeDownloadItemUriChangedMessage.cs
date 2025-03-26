using CommunityToolkit.Mvvm.Messaging.Messages;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.ViewModels;

public class YoutubeDownloadItemUriChangedMessage : ValueChangedMessage<YoutubeDownloadItem>
{
    public YoutubeDownloadItemUriChangedMessage(YoutubeDownloadItem item) : base(item) { }
}
