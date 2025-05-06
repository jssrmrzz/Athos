using Athos.ReviewAutomation.Application.UseCases.Reviews;
using Athos.ReviewAutomation.Core.Entities;
using Athos.ReviewAutomation.Core.Interfaces;
using Athos.ReviewAutomation.Core.Services;
using Athos.ReviewAutomation.Infrastructure.Data;
using Athos.ReviewAutomation.Infrastructure.Repositories;
using Athos.ReviewAutomation.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Configure SQLite with migrations assembly
builder.Services.AddDbContext<ReviewDbContext>(options =>
    options.UseSqlite("Data Source=reviews.db", b =>
        b.MigrationsAssembly("Athos.ReviewAutomation.Infrastructure")));

// Register infrastructure services
builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<GoogleReviewClient>();

// Register internal services
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<AutoReplyService>();
builder.Services.AddScoped<SentimentService>();
builder.Services.AddScoped<ReviewApprovalService>();
builder.Services.AddScoped<ReviewPollingService>();
builder.Services.AddScoped<GoogleReviewIngestionService>();

// Register use case interfaces
builder.Services.AddScoped<IGetReviewsUseCase, GetReviewsUseCase>();
builder.Services.AddScoped<IApproveReviewUseCase, ApproveReviewUseCase>();
builder.Services.AddScoped<IReviewPollingService, ReviewPollingService>();
builder.Services.AddScoped<IGoogleReviewIngestionService, GoogleReviewIngestionService>();

// Configure HttpClient with Polly retry policy
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

// Define retry policy for transient HTTP failures
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds}s due to {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
            });
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
