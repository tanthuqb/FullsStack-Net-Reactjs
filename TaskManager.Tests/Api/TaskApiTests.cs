using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using TaskManager.Models;
using TaskManager.Tests.TestUtilities;

namespace TaskManager.Tests
{
    public class TaskApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public TaskApiTests(CustomWebApplicationFactory f) => _client = f.CreateClient();

        [Fact]
        public async Task PostAndGet_TaskEntity_ReturnsSuccess()
        {
            var inTask = new TaskEntity { Title = "xxx", Description = "desc" };
            var post = await _client.PostAsJsonAsync("/api/TaskEntity", inTask);
            post.EnsureSuccessStatusCode();

            var body = await post.Content.ReadAsStringAsync();
            var got = JsonSerializer.Deserialize<TaskEntity>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(got);
            Assert.True(got.Id > 0);

            var get = await _client.GetAsync($"/api/TaskEntity/{got.Id}");
            get.EnsureSuccessStatusCode();
            var fetched = await get.Content.ReadFromJsonAsync<TaskEntity>();
            Assert.Equal("xxx", fetched!.Title);
        }
    }
}
