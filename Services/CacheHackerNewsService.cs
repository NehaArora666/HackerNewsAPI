using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using HackerNewsAPI.Models;

public class CachedHackerNewsService : ICachedHackerNewsService
{
    private readonly IHackerNewsService _innerService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedHackerNewsService> _logger;

    private static readonly string _topStoriesCacheKey = "TopStoriesIds";

    public CachedHackerNewsService(IHackerNewsService innerService, IMemoryCache cache, ILogger<CachedHackerNewsService> logger)
    {
        _innerService = innerService;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<StoryModel>> GetTopStoriesAsync()
    {
        try
        {
            var topStoryIds = await GetCachedTopStoryIdsAsync();
            var selectedIds = topStoryIds.Take(200);

            var tasks = selectedIds.Select(id => GetCachedStoryDetailsAsync(id)).ToList();
            var fetchedStories = await Task.WhenAll(tasks);

            return fetchedStories.Where(s => s != null).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get top stories with caching.");
            return new List<StoryModel>();
        }
    }

    private async Task<List<int>> GetCachedTopStoryIdsAsync()
    {
        if (_cache.TryGetValue(_topStoriesCacheKey, out List<int> cachedIds))
        {
            return cachedIds;
        }

        var storyIds = await _innerService.FetchTopStoryIdsAsync();

        _cache.Set(_topStoriesCacheKey, storyIds, TimeSpan.FromMinutes(1));
        return storyIds;
    }

    private async Task<StoryModel> GetCachedStoryDetailsAsync(int storyId)
    {
        if (_cache.TryGetValue(storyId, out StoryModel cachedStory))
        {
            return cachedStory;
        }

        var story = await _innerService.FetchStoryDetailsAsync(storyId);

        if (story != null)
        {
            _cache.Set(storyId, story, TimeSpan.FromMinutes(5));
        }

        return story;
    }
}
