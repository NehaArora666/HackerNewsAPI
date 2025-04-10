using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HackerNewsAPI.Models;

public class HackerNewsService : IHackerNewsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HackerNewsService> _logger;
    private readonly string _topStoriesUrl = "https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty";
    private readonly string _itemUrlTemplate = "https://hacker-news.firebaseio.com/v0/item/{0}.json?print=pretty";

    public HackerNewsService(HttpClient httpClient, ILogger<HackerNewsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<int>> FetchTopStoryIdsAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync(_topStoriesUrl);
            return JsonConvert.DeserializeObject<List<int>>(response)?.Take(200).ToList() ?? new List<int>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching top story IDs.");
            return new List<int>();
        }
    }

    public async Task<StoryModel> FetchStoryDetailsAsync(int storyId)
    {
        try
        {
            var response = await _httpClient.GetStringAsync(string.Format(_itemUrlTemplate, storyId));
            return JsonConvert.DeserializeObject<StoryModel>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching story ID {StoryId}", storyId);
            return null;
        }
    }
}
