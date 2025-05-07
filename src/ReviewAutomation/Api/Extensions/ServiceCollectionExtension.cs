using System.Net.Http;
using Athos.ReviewAutomation.Application.UseCases;
using Athos.ReviewAutomation.Application.UseCases.Reviews;
using Athos.ReviewAutomation.Core.Entities;
using Athos.ReviewAutomation.Core.Interfaces;
using Athos.ReviewAutomation.Core.Services;
using Athos.ReviewAutomation.Infrastructure.Data;
using Athos.ReviewAutomation.Infrastructure.Repositories;
using Athos.ReviewAutomation.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Athos.ReviewAutomation.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ReviewDbContext>(options =>
                options.UseSqlite(connectionString, b =>
                    b.MigrationsAssembly("Athos.ReviewAutomation.Infrastructure")));

            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<NotificationService>();
            services.AddScoped<AutoReplyService>();
            services.AddScoped<SentimentService>();

            services.AddHttpClient<GoogleReviewClient>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7157/");
                })
                .AddPolicyHandler(GetRetryPolicy());

            services.AddScoped<IGoogleReviewIngestionService, GoogleReviewIngestionService>();
            services.AddScoped<IReviewPollingService, ReviewPollingService>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IGetReviewsUseCase, GetReviewsUseCase>();
            services.AddScoped<IApproveReviewUseCase, ApproveReviewUseCase>();

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
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
    }
}
