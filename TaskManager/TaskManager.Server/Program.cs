using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TaskManager.Context;
using TaskManager.Data;
using TaskManager.Models;


var builder = WebApplication.CreateBuilder(args);

// 💡 Add DB context
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// 💡 Add Authentication & Authorization
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/";
        options.LogoutPath = "/api/auth/logout";
        options.Cookie.Name = "MyAppCookie";
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401; 
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

// 💡 Add Swagger support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Manager API",
        Version = "v1",
        Description = "API for user authentication and task management"
    });
});

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// 💡 Add Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

// 💡 Auto-migrate DB
if (!app.Environment.IsEnvironment("Testing")) 
{ 
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (db.Database.IsRelational())
        {
            db.Database.Migrate();
        }

        DbSeeder.Seed(db);
    }
}

// 💡 Enable Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Manager API v1");
    c.RoutePrefix = "swagger"; // ⬅️ Truy cập tại: http://localhost:5000/swagger
});

// 💡 Optional: skip HTTPS redirect error during dev
// app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// 💡 Enable Controllers
app.MapControllers();

// 💡 Map minimal API if có
app.MapTaskEntityEndpoints();
app.MapUserEntityEndpoints();

// 💡 Default fallback
//app.MapFallbackToFile("/index.html");

app.Run();
public partial class Program { }