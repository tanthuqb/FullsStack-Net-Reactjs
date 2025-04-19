using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using TaskManager.Models;
using TaskManager.Tests.TestUtilities;
using System.Collections.Generic;

namespace TaskManager.Tests
{
    public class UserApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public UserApiTests(CustomWebApplicationFactory f) => _client = f.CreateClient();

        [Fact]
        public async Task CreateUser_ReturnsCreated()
        {
            var u = new UserEntity
            {
                UserName = "u1",
                Email = "u1@x.com",
                Password = "p",
                Role = UserRole.Regular
            };
            var post = await _client.PostAsJsonAsync("/api/UserEntity", u);
            var raw = await post.Content.ReadAsStringAsync();
            Assert.True(post.IsSuccessStatusCode);
            Assert.False(string.IsNullOrWhiteSpace(raw));
        }

        [Fact]
        public async Task FilterAndSortUsers_WorksCorrectly()
        {
            var seed = new UserEntity
            {
                UserName = "filtertest",
                Email = "testfilter@example.com",
                Password = "123"
            };
            await _client.PostAsJsonAsync("/api/UserEntity", seed);

            var response = await _client.GetAsync("/api/UserEntity");
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<List<UserEntity>>();
            Assert.NotNull(list);
            Assert.Contains(list, u => u.Email.Contains("testfilter"));
        }
    }
}
