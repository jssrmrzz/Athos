using Athos.ReviewAutomation.Core.Services;
using Athos.ReviewAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Athos.ReviewAutomation.Infrastructure.Repositories;
using Athos.ReviewAutomation.Infrastructure.Services;
using Athos.ReviewAutomation.Models;
    


var builder = WebApplication.CreateBuilder(args);

// Connects to SQLite file
builder.Services.AddDbContext<ReviewDbContext>(options =>
    options.UseSqlite("Data Source=reviews.db"));

// Register EF Core DbContext
builder.Services.AddDbContext<ReviewDbContext>();

// Register services
builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<ReviewPollingService>();
builder.Services.AddScoped<ReviewApprovalService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<AutoReplyService>();
builder.Services.AddScoped<SentimentService>();

// Add controllers, Swagger, etc
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ReviewDbContext>(options =>
    options.UseSqlite("Data Source=reviews.db", b =>
        b.MigrationsAssembly("Athos.ReviewAutomation.Infrastructure")));



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
// 🔥 Trigger mock review seeding at startup
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<ReviewRepository>();
    repo.SeedReviewsFromJsonIfEmpty();
}

app.Run();