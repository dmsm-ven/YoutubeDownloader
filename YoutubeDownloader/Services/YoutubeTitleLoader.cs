using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

namespace YoutubeDownloader.Services;

public class YoutubeTitleResolver
{
    private readonly HttpClient client;

    public YoutubeTitleResolver(HttpClient client)
    {
        this.client = client;
    }

    public async Task<string> GetTitleByUri(string uri)
    {
        try
        {
            string id = HttpUtility.ParseQueryString(new Uri(uri).Query).Get("v");
            string api = $"https://www.youtube.com/oembed?url=youtube.com/watch?v={id}&format=json";
            var apiResponse = await client.GetFromJsonAsync<YoutubeOembedResponse>(api);

            string clearTitle = new(apiResponse.title.Except(Path.GetInvalidFileNameChars()).ToArray());

            return !string.IsNullOrWhiteSpace(clearTitle) ? clearTitle : "Ошибка API";
        }
        catch
        {
        }

        return null;
    }

    public class YoutubeOembedResponse
    {
        public string title { get; set; }
        public string author_name { get; set; }
        public string author_url { get; set; }
        //public string type { get; set; }
        //public int height { get; set; }
        //public int width { get; set; }
        //public string version { get; set; }
        //public string provider_name { get; set; }
        //public string provider_url { get; set; }
        //public int thumbnail_height { get; set; }
        //public int thumbnail_width { get; set; }
        //public string thumbnail_url { get; set; }
        //public string html { get; set; }
    }
}

