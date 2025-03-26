using CommunityToolkit.Mvvm.Messaging.Messages;

namespace YoutubeDownloader.Models;

public class YoutubeDownloadItemDeletedMessage : ValueChangedMessage<YoutubeDownloadItem>
{
    public YoutubeDownloadItemDeletedMessage(YoutubeDownloadItem item) : base(item) { }
}
