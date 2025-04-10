using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Protected;
using System.Threading;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using HackerNewsAPI.Models;
using System.Collections.Generic;

public class HackerNewsServiceTests
{
    private HackerNewsService CreateService(string responseContent)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent),
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var loggerMock = new Mock<ILogger<HackerNewsService>>();

        return new HackerNewsService(httpClient, loggerMock.Object);
    }

    [Fact]
    public async Task FetchTopStoryIdsAsync_ReturnsList()
    {
        var json = JsonConvert.SerializeObject(new List<int> { 1, 2, 3 });
        var service = CreateService(json);

        var result = await service.FetchTopStoryIdsAsync();

        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task FetchStoryDetailsAsync_ReturnsStory()
    {
        var json = JsonConvert.SerializeObject(new StoryModel { Url = "www.test.com", Title = "Test Story" });
        var service = CreateService(json);

        var result = await service.FetchStoryDetailsAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Test Story", result.Title);
    }
}
