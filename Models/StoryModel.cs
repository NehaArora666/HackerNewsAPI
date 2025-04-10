namespace HackerNewsAPI.Models
{
    public class StoryModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
    public interface IHackerNewsService
    {
        Task<List<int>> FetchTopStoryIdsAsync();
        Task<StoryModel> FetchStoryDetailsAsync(int storyId);
    }
    public interface ICachedHackerNewsService
    {
        Task<List<StoryModel>> GetTopStoriesAsync();
    }
}