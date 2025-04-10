using Xunit;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using HackerNewsAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CachedHackerNewsServiceTests
{
    private readonly Mock<IHackerNewsService> _mockService = new();
    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
    private readonly Mock<ILogger<CachedHackerNewsService>> _logger = new();

    [Fact]
    public async Task GetTopStoriesAsync_ReturnsCachedData()
    {
        var storyA = new StoryModel { Title = "story A", Url = "https://a.com" };
        var storyB = new StoryModel { Title = "story B", Url = "https://b.com" };

        _mockService.Setup(x => x.FetchTopStoryIdsAsync())
            .ReturnsAsync(new List<int> { 101, 102 });

        _mockService.Setup(x => x.FetchStoryDetailsAsync(101)).ReturnsAsync(storyA);
        _mockService.Setup(x => x.FetchStoryDetailsAsync(102)).ReturnsAsync(storyB);

        var cachedService = new CachedHackerNewsService(_mockService.Object, _memoryCache, _logger.Object);

        var result = await cachedService.GetTopStoriesAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Title == "Story A");
        Assert.Contains(result, s => s.Url == "https://b.com");

        
        var secondFetch = await cachedService.GetTopStoriesAsync();

        _mockService.Verify(x => x.FetchTopStoryIdsAsync(), Times.Once);
        _mockService.Verify(x => x.FetchStoryDetailsAsync(101), Times.Once); 
    }
}
