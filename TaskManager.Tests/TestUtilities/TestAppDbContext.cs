using Microsoft.EntityFrameworkCore;
using TaskManager.Context;

namespace TaskManager.Tests.TestUtilities
{
    public class TestAppDbContext : AppDbContext
    {
        public TestAppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }
    }
}