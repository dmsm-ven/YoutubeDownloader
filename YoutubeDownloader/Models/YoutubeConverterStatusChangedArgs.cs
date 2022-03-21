namespace YoutubeDownloader;

public class YoutubeConverterStatusChangedArgs
{
    public LoadStatus Status { get; }
    public string Message { get; }
    public string Uri { get; }
    public string LocalName { get; }

    public YoutubeConverterStatusChangedArgs(LoadStatus status, string message, string uri, string localName)
    {
        Status = status;
        Message = message;
        Uri = uri;
        LocalName = localName;
    }
}
