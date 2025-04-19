using TaskManager.Context;
using TaskManager.Models;

namespace TaskManager.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // Seed Users
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new UserEntity
                    {
                        UserName = "admin",
                        Email = "admin@example.com",
                        Password = "admin123",
                        Role = UserRole.Admin
                    },
                    new UserEntity
                    {
                        UserName = "user",
                        Email = "user@example.com",
                        Password = "user123",
                        Role = UserRole.Regular
                    }
                );
            }

            // Seed Tasks
            if (!context.TaskEntities.Any())
            {
                context.TaskEntities.AddRange(
                    new TaskEntity
                    {
                        Title = "Initial Setup",
                        Description = "Setup the project structure and database.",
                        Status = TaskEnumStatus.Completed,
                        DueDate = DateTime.UtcNow.AddDays(3)
                    },
                    new TaskEntity
                    {
                        Title = "Implement Authentication",
                        Description = "Add login and register endpoints.",
                        Status = TaskEnumStatus.InProgress,
                        DueDate = DateTime.UtcNow.AddDays(5)
                    },
                    new TaskEntity
                    {
                        Title = "Seed Sample Data",
                        Description = "Seed initial users and tasks for development.",
                        Status = TaskEnumStatus.NotStarted,
                        DueDate = DateTime.UtcNow.AddDays(7)
                    }
                );
            }

            context.SaveChanges();
        }
    }
}
