using Microsoft.AspNetCore.Mvc;
using TopStoriesApi.Services;

namespace TopStoriesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoriesController : ControllerBase
    {
        private readonly IService _service;

        public StoriesController(IService service)
        {
            _service = service;
        }

        [HttpGet("best")]
        public async Task<IActionResult> GetTopStories([FromQuery] int n = 10)
        {
            var stories = await _service.GetTopStoriesAsync(n: n);

            return Ok(stories);
        }

    }
}
