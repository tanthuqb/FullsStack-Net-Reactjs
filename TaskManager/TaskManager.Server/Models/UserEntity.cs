using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TaskManager.Context;

namespace TaskManager.Models
{
    public enum UserRole
    {
        Admin,
        Regular
    }
    public class UserEntity
    {
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Regular;

    }


    public static class UserEntityEndpoints
    {
        public static void MapUserEntityEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/UserEntity").RequireAuthorization().WithTags(nameof(UserEntity));

            group.MapGet("/", async (AppDbContext db) =>
            {
                return await db.Users.ToListAsync();
            })
            .WithName("GetAllUserEntities")
            .WithOpenApi();

            group.MapGet("/{id}", async Task<Results<Ok<UserEntity>, NotFound>> (int id, AppDbContext db) =>
            {
                return await db.Users.AsNoTracking()
                    .FirstOrDefaultAsync(model => model.Id == id)
                    is UserEntity model
                        ? TypedResults.Ok(model)
                        : TypedResults.NotFound();
            })
            .WithName("GetUserEntityById")
            .WithOpenApi();

            group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, UserEntity userEntity, AppDbContext db) =>
            {
                var affected = await db.Users
                    .Where(model => model.Id == id)
                    .ExecuteUpdateAsync(setters => setters
                      .SetProperty(m => m.Id, userEntity.Id)
                      .SetProperty(m => m.UserName, userEntity.UserName)
                      .SetProperty(m => m.Password, userEntity.Password)
                      .SetProperty(m => m.Email, userEntity.Email)
                      );
                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
            .WithName("UpdateUserEntity")
            .WithOpenApi();

            group.MapPost("/", async (UserEntity userEntity, AppDbContext db) =>
            {
                db.Users.Add(userEntity);
                await db.SaveChangesAsync();
                return TypedResults.Created($"/api/UserEntity/{userEntity.Id}", userEntity);
            })
            .WithName("CreateUserEntity")
            .WithOpenApi();

            group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
            {
                var affected = await db.Users
                    .Where(model => model.Id == id)
                    .ExecuteDeleteAsync();
                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
            .WithName("DeleteUserEntity")
            .WithOpenApi();

            group.MapGet("/api/users/filter", async (string? email, string? sort, AppDbContext db) =>
            {
                var query = db.Users.AsQueryable();

                if (!string.IsNullOrEmpty(email))
                {
                    query = query.Where(u => u.Email.Contains(email));
                }

                if (sort?.ToLower() == "asc")
                {
                    query = query.OrderBy(u => u.UserName);
                }
                else if (sort?.ToLower() == "desc")
                {
                    query = query.OrderByDescending(u => u.UserName);
                }

                return await query.ToListAsync();
            })
            .WithName("FilterAndSortUsers")
            .WithOpenApi();          
        }
    }
}