using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;
using TaskManager.Context;

namespace TaskManager.Tests
{
    public class TaskEntityTests
    {
        private static AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
         .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
         .Options;

            return new AppDbContext(options);

        }

        [Fact]
        public async Task CreateTaskEntity_SavesToDb()
        {
            var db = GetDbContext();
            var task = new TaskEntity { Title = "Test", Description = "Test Desc" };
            db.TaskEntities.Add(task);
            await db.SaveChangesAsync();

            Assert.Equal(1, await db.TaskEntities.CountAsync());
            var first = await db.TaskEntities.FirstAsync();
            Assert.Equal("Test", first.Title);
        }

        [Fact]
        public async Task FilterByStatus_ReturnsCorrectTasks()
        {
            var db = GetDbContext();
            db.TaskEntities.Add(new TaskEntity { Title = "T1", Status = TaskEnumStatus.ToDo });
            db.TaskEntities.Add(new TaskEntity { Title = "T2", Status = TaskEnumStatus.Completed });
            await db.SaveChangesAsync();

            var result = await db.TaskEntities.Where(t => t.Status == TaskEnumStatus.ToDo).ToListAsync();

            Assert.Single(result);
            Assert.Equal("T1", result.First().Title);
        }
    }

}