using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using SpotifyClone.Models;

namespace SpotifyClone.Controllers;

public class HomeController : Controller
{
    private readonly string _apiKey;

    private readonly AppDbContext _context;

    public HomeController(IConfiguration configuration, AppDbContext context)
    {
        _apiKey = configuration["APIKey"] ?? string.Empty;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        string url = $"https://www.googleapis.com/youtube/v3/videos?part=snippet,contentDetails&chart=mostPopular&videoCategoryId=10&regionCode=US&maxResults=100&key={_apiKey}";

        using var client = new HttpClient();
        var json = await client.GetStringAsync(url);

        // Parse JSON dynamically
        var node = JsonNode.Parse(json);
        var items = node?["items"].AsArray();

        var videos = new List<YouTubeVideo>();
        foreach (var item in items)
        {
            videos.Add(new YouTubeVideo
            {
                VideoId = item["id"].ToString(),
                Title = item["snippet"]["title"].ToString(),
                Duration = FormatDuration(
                    DurationToSeconds(item["contentDetails"]["duration"].ToString())
                )
            });
        }

        var playlists = _context.Playlists.ToList();
        ViewData["Playlists"] = playlists;

        return View(videos);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query cannot be empty.");

        using var client = new HttpClient();
        string url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&type=video&maxResults=5&q={Uri.EscapeDataString(query)}&key={_apiKey}";

        var json = await client.GetStringAsync(url);
        var node = JsonNode.Parse(json);
        var items = node?["items"]?.AsArray();

        if (items == null)
            return Json(Array.Empty<object>());

        var results = new List<YouTubeVideo>();

        foreach (var item in items)
        {
            string videoId = item["id"]["videoId"]?.ToString() ?? "";
            string title = item["snippet"]["title"]?.ToString() ?? "";
            string thumbnailUrl = item["snippet"]["thumbnails"]?["medium"]?["url"]?.ToString() ?? "";

            // Optional: Fetch duration using Videos API
            int durationSeconds = await GetVideoDurationSeconds(videoId);

            results.Add(new YouTubeVideo
            {
                VideoId = videoId,
                Title = title,
                Duration = FormatDuration(durationSeconds), // "mm:ss"
                ThumbnailUrl = thumbnailUrl
            });
        }

        return Json(results);
    }

    private async Task<int> GetVideoDurationSeconds(string videoId)
    {
        if (string.IsNullOrEmpty(videoId)) return 0;

        using var client = new HttpClient();
        string url = $"https://www.googleapis.com/youtube/v3/videos?part=contentDetails&id={videoId}&key={_apiKey}";
        var json = await client.GetStringAsync(url);
        var node = JsonNode.Parse(json);
        var item = node?["items"]?.AsArray()?.FirstOrDefault();
        if (item == null) return 0;

        string isoDuration = item["contentDetails"]["duration"]?.ToString() ?? "PT0S";
        var ts = System.Xml.XmlConvert.ToTimeSpan(isoDuration);
        return (int)ts.TotalSeconds;
    }

    public int DurationToSeconds(string isoDuration)
    {
        var duration = XmlConvert.ToTimeSpan(isoDuration);
        return (int)duration.TotalSeconds;
    }

    public string FormatDuration(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes}:{seconds:D2}";
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class YouTubeVideo
{
    public string VideoId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Duration { get; set; } = null!;
    public string ThumbnailUrl { get; set; } = null!;
}
