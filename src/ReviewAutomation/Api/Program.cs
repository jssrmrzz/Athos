using Athos.ReviewAutomation.Core.Services;
using Athos.ReviewAutomation.Infrastructure.Data;
using Athos.ReviewAutomation.Infrastructure.Repositories;
using Athos.ReviewAutomation.Infrastructure.Services;
using Athos.ReviewAutomation.Models;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Connect to SQLite with Migrations assembly set
builder.Services.AddDbContext<ReviewDbContext>(options =>
    options.UseSqlite("Data Source=reviews.db", b =>
        b.MigrationsAssembly("Athos.ReviewAutomation.Infrastructure")));

// Register internal services
builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<ReviewPollingService>();
builder.Services.AddScoped<ReviewApprovalService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<AutoReplyService>();
builder.Services.AddScoped<SentimentService>();
builder.Services.AddScoped<GoogleReviewIngestionService>();

// Register HttpClient for GoogleReviewClient with Polly retry logic
builder.Services.AddHttpClient<GoogleReviewClient>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:7157/");
    })
    .AddPolicyHandler(GetRetryPolicy());

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Define Polly retry policy
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
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
app.Run();
