using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using HackerNewsAPI.Models;
using HackerNewsAPI.Controllers;
using Newtonsoft.Json.Linq;


public class HackerNewsControllerTests
{
    [Fact]
    public async Task GetTopStories_ReturnsPaginatedOkResult()
    {
        
        var mockService = new Mock<ICachedHackerNewsService>();

        var allStories = new List<StoryModel>
        {
            new StoryModel { Title = "Story 1", Url = "https://story1.com" },
            new StoryModel { Title = "Story 2", Url = "https://story2.com" },
            new StoryModel { Title = "Story 3", Url = "https://story3.com" }
        };

        mockService
            .Setup(x => x.GetTopStoriesAsync())
            .ReturnsAsync(allStories);

        var controller = new HackerNewsController(mockService.Object);

        int page = 0;
        int pageSize = 2;

       
        var result = await controller.GetTopStories(page, pageSize);

        
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = JObject.FromObject(okResult.Value);

        var items = response["items"].ToObject<List<StoryModel>>(); 
        var totalCount = response["totalCount"].Value<int>(); 

       
        Assert.Equal(2, items.Count);
        Assert.Equal(3, totalCount); 

       
        Assert.Equal("Story 1", items[0].Title);
        Assert.Equal("https://story1.com", items[0].Url);
    }
}








