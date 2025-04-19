using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;
using TaskManager.Context;

public class UserEntityTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb_Users")
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateUser_SavesCorrectly()
    {
        var db = GetDbContext();
        var user = new UserEntity
        {
            UserName = "andy",
            Email = "a@a.com",
            Password = "123"
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var count = await db.Users.CountAsync();
        Assert.Equal(1, count);

        var first = await db.Users.FirstAsync();
        Assert.Equal("andy", first.UserName);
    }
}
