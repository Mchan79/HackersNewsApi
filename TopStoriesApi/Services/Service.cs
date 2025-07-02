using Microsoft.Extensions.Caching.Memory;
using TopStoriesApi.Models;

namespace TopStoriesApi.Services
{
    public class Service : IService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private const string BestStoriesUrl = "https://hacker-news.firebaseio.com/v0/beststories.json";
        private const string ItemUrlTemplate = "https://hacker-news.firebaseio.com/v0/item/{0}.json";

        public Service(HttpClient httpClient, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _cache = memoryCache;
        }

        public async Task<List<StoryDto?>> GetTopStoriesAsync(int n)
        {
            var storyIds = await _cache.GetOrCreateAsync("beststories", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
                return await _httpClient.GetFromJsonAsync<List<int>>(BestStoriesUrl);
            });

            var tasks = storyIds
                .Take(100)
                .Select(id => GetStoryByIdAsync(id));

            var stories = await Task.WhenAll(tasks);
            return stories
                .Where(s => s != null)
                .OrderByDescending(s => s.Score)
                .Take(n)
                .ToList();
        }

        private async Task<StoryDto?> GetStoryByIdAsync(int id)
        {
            return await _cache.GetOrCreateAsync($"story_{id}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                var item = await _httpClient.GetFromJsonAsync<Item>(string.Format(ItemUrlTemplate, id));
                if (item == null || string.IsNullOrWhiteSpace(item.Title)) return null;

                return new StoryDto
                {
                    Title = item.Title,
                    Uri = item.Url,
                    PostedBy = item.By,
                    Time = DateTimeOffset.FromUnixTimeSeconds(item.Time).UtcDateTime,
                    Score = item.Score,
                    CommentCount = item.Descendants
                };
            });
        }

    }

    public interface IService
    {
        Task<List<StoryDto?>> GetTopStoriesAsync(int n);
    }
}
