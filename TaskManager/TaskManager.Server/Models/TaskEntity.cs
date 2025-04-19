using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using TaskManager.Context;

namespace TaskManager.Models
{
    public enum TaskEnumStatus
    {
        ToDo,
        InProgress,
        Completed,
        NotStarted
    }

    public class TaskEntity
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskEnumStatus Status { get; set; } = TaskEnumStatus.NotStarted;

        public DateTime? DueDate { get; set; }
    }

    public static class TaskEntityEndpoints
    {
        public static void MapTaskEntityEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/TaskEntity")
                .RequireAuthorization()
                .WithTags(nameof(TaskEntity));

            group.MapGet("/", async (HttpContext context) =>
            {
                var db = context.RequestServices.GetRequiredService<AppDbContext>();
                var page = int.TryParse(context.Request.Query["page"], out var p) ? p : 1;
                var pageSize = int.TryParse(context.Request.Query["pageSize"], out var ps) ? ps : 10;
                var skip = (page - 1) * pageSize;

                var tasks = await db.TaskEntities
                    .OrderByDescending(t => t.Id)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                return Results.Ok(tasks);
            }).WithName("GetAllTaskEntities").WithOpenApi();

            group.MapGet("/{id}", async (AppDbContext db, int id) =>
            {
                var task = await db.TaskEntities.AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id);

                return task is not null ? Results.Ok(task) : Results.NotFound();
            }).WithName("GetTaskEntityById").WithOpenApi();

            group.MapPost("/", async (AppDbContext db, TaskEntity taskEntity) =>
            {
                if (taskEntity.DueDate.HasValue)
                    taskEntity.DueDate = DateTime.SpecifyKind(taskEntity.DueDate.Value, DateTimeKind.Utc);

                db.TaskEntities.Add(taskEntity);
                await db.SaveChangesAsync();

                return Results.Created($"/api/TaskEntity/{taskEntity.Id}", taskEntity);
            }).WithName("CreateTaskEntity").WithOpenApi();

            group.MapPut("/{id}", async (AppDbContext db, int id, TaskEntity taskEntity) =>
            {
                if (taskEntity.DueDate.HasValue)
                    taskEntity.DueDate = DateTime.SpecifyKind(taskEntity.DueDate.Value, DateTimeKind.Utc);

                var affected = await db.TaskEntities
                    .Where(t => t.Id == id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(t => t.Title, taskEntity.Title)
                        .SetProperty(t => t.Description, taskEntity.Description)
                        .SetProperty(t => t.Status, taskEntity.Status)
                        .SetProperty(t => t.DueDate, taskEntity.DueDate));

                return affected == 1 ? Results.Ok() : Results.NotFound();
            }).WithName("UpdateTaskEntity").WithOpenApi();

            group.MapDelete("/{id}", async (AppDbContext db, int id) =>
            {
                var affected = await db.TaskEntities
                    .Where(t => t.Id == id)
                    .ExecuteDeleteAsync();

                return affected == 1 ? Results.Ok() : Results.NotFound();
            }).WithName("DeleteTaskEntity").WithOpenApi();

            group.MapGet("/filter", async (HttpContext context) =>
            {
                var db = context.RequestServices.GetRequiredService<AppDbContext>();
                var status = context.Request.Query["status"].ToString();
                var sort = context.Request.Query["sort"].ToString();
                var page = int.TryParse(context.Request.Query["page"], out var p) ? p : 1;
                var pageSize = int.TryParse(context.Request.Query["pageSize"], out var ps) ? ps : 10;

                var query = db.TaskEntities.AsQueryable();

                if (!string.IsNullOrEmpty(status) && Enum.TryParse<TaskEnumStatus>(status, true, out var parsedStatus))
                    query = query.Where(t => t.Status == parsedStatus);

                if (sort == "asc") query = query.OrderBy(t => t.DueDate);
                else if (sort == "desc") query = query.OrderByDescending(t => t.DueDate);

                var skip = (page - 1) * pageSize;
                var result = await query.Skip(skip).Take(pageSize).ToListAsync();

                return Results.Ok(result);
            }).WithName("FilterAndSortTasks").WithOpenApi();

            group.MapGet("/search", async (HttpContext context) =>
            {
                var db = context.RequestServices.GetRequiredService<AppDbContext>();
                var keyword = context.Request.Query["keyword"].ToString();
                var page = int.TryParse(context.Request.Query["page"], out var p) ? p : 1;
                var pageSize = int.TryParse(context.Request.Query["pageSize"], out var ps) ? ps : 10;

                var query = db.TaskEntities.AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                    query = query.Where(t => t.Title.Contains(keyword));

                var skip = (page - 1) * pageSize;
                var result = await query.OrderByDescending(t => t.Id)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                return Results.Ok(result);
            }).WithName("SearchTasksByTitle").WithOpenApi();

            group.MapGet("/filter-advanced", async (HttpContext context) =>
            {
                var db = context.RequestServices.GetRequiredService<AppDbContext>();
                var status = context.Request.Query["status"].ToString();
                var sort = context.Request.Query["sort"].ToString();
                var keyword = context.Request.Query["keyword"].ToString();
                var page = int.TryParse(context.Request.Query["page"], out var p) ? p : 1;
                var pageSize = int.TryParse(context.Request.Query["pageSize"], out var ps) ? ps : 10;

                var query = db.TaskEntities.AsQueryable();

                if (!string.IsNullOrEmpty(status) && Enum.TryParse<TaskEnumStatus>(status, true, out var parsedStatus))
                    query = query.Where(t => t.Status == parsedStatus);

                if (!string.IsNullOrWhiteSpace(keyword))
                    query = query.Where(t => t.Title.Contains(keyword));

                if (sort == "asc") query = query.OrderBy(t => t.DueDate);
                else if (sort == "desc") query = query.OrderByDescending(t => t.DueDate);

                var skip = (page - 1) * pageSize;
                var result = await query.Skip(skip).Take(pageSize).ToListAsync();

                return Results.Ok(result);
            }).WithName("AdvancedFilterSortSearchTasks").WithOpenApi();
        }
    }
}
