using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using HackerNewsAPI.Models;
using Newtonsoft.Json;
public class HackerNewsService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<HackerNewsService> _logger;
    private readonly string _topStoriesUrl = "https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty";
    private readonly string _itemUrlTemplate = "https://hacker-news.firebaseio.com/v0/item/{0}.json?print=pretty";

    public HackerNewsService(HttpClient httpClient, IMemoryCache cache, ILogger<HackerNewsService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<StoryModel>> GetTopStoriesAsync()
    {
        try
        {
            var topStoryIds = await FetchTopStoryIdsAsync();

            var stories = new List<StoryModel>();
            var tasks = topStoryIds.Select(id => FetchStoryDetailsAsync(id)).ToList();
            var fetchedStories = await Task.WhenAll(tasks);
            stories.AddRange(fetchedStories);

            return stories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching top stories.");
            return new List<StoryModel>();
        }
    }

    public async Task<List<int>> FetchTopStoryIdsAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync(_topStoriesUrl);
            var storyIds = JsonConvert.DeserializeObject<List<int>>(response);
            return storyIds.Take(200).ToList();
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
            _logger.LogError(ex, "Error fetching details for story ID: {StoryId}", storyId);
            return null;
        }
    }
}
