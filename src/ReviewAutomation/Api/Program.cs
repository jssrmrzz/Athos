using Athos.ReviewAutomation.Core.Services;
using Athos.ReviewAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Athos.ReviewAutomation.Infrastructure.Repositories;
using Athos.ReviewAutomation.Infrastructure.Services;
using Athos.ReviewAutomation.Models;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Connect to SQLite
builder.Services.AddDbContext<ReviewDbContext>(options =>
    options.UseSqlite("Data Source=reviews.db", b =>
        b.MigrationsAssembly("Athos.ReviewAutomation.Infrastructure")));

// Register services
builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<ReviewPollingService>();
builder.Services.AddScoped<ReviewApprovalService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<AutoReplyService>();
builder.Services.AddScoped<SentimentService>();

// Register HttpClient with Polly retry
builder.Services.AddHttpClient<GoogleReviewClient>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:7157/");
    })
    .AddPolicyHandler(GetRetryPolicy());

// Add controllers, Swagger, etc.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Polly retry policy method
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // 2s, 4s, 8s
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                Console.WriteLine($"⚠️ Retry {retryAttempt} after {timespan.TotalSeconds}s due to {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
            });
}


// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// Seed local JSON if DB is empty
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<ReviewRepository>();
    repo.SeedReviewsFromJsonIfEmpty();
}

app.Run();