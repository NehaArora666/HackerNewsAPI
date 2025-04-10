using HackerNewsAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HackerNewsController : ControllerBase
    {
        private readonly ICachedHackerNewsService _hackerNewsService;

        public HackerNewsController(ICachedHackerNewsService hackerNewsService)
        {
            _hackerNewsService = hackerNewsService;
        }

        [HttpGet("top-stories")]
        public async Task<IActionResult> GetTopStories([FromQuery] int page = 0, [FromQuery] int pageSize = 10)
        {
            try
            {
                var topStories = await _hackerNewsService.GetTopStoriesAsync();
                var paginatedStories = topStories
                    .Skip(page * pageSize) 
                    .Take(pageSize) 
                    .ToList();
                var totalCount = topStories.Count();
                return Ok(new { items = paginatedStories, totalCount = totalCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}