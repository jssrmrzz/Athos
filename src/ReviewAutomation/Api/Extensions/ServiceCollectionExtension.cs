using System.Net.Http;
using Athos.ReviewAutomation.Application.UseCases;
using Athos.ReviewAutomation.Application.UseCases.Reviews;
using Athos.ReviewAutomation.Core.Entities;
using Athos.ReviewAutomation.Core.Interfaces;
using Athos.ReviewAutomation.Core.Services;
using Athos.ReviewAutomation.Infrastructure.Data;
using Athos.ReviewAutomation.Infrastructure.LLM;
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
        /// <summary>
        /// Configures infrastructure dependencies, database, repositories, services, and the active LLM client.
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString, IConfiguration config)
        {
            // Register EF Core using SQLite with migration support
            services.AddDbContext<ReviewDbContext>(options =>
                options.UseSqlite(connectionString, b =>
                    b.MigrationsAssembly("Athos.ReviewAutomation.Infrastructure")));

            // Core infrastructure services and repositories
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<NotificationService>();
            services.AddScoped<AutoReplyService>();
            services.AddScoped<SentimentService>();
            services.AddScoped<IReviewApprovalService, ReviewApprovalService>();
            services.AddScoped<IGoogleReviewIngestionService, GoogleReviewIngestionService>();
            services.AddScoped<IReviewPollingService, ReviewPollingService>();
            services.AddScoped<GoogleReviewClient>();

            // Register HTTP clients with retry policy
            services.AddHttpClient("OpenAI", client =>
            {
                client.BaseAddress = new Uri("https://api.openai.com/v1/");
            }).AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient("LocalLLM", client =>
            {
                client.BaseAddress = new Uri("http://localhost:11434/"); // Default for Ollama
            }).AddPolicyHandler(GetRetryPolicy());
            
            services.AddHttpClient<GoogleReviewClient>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7157/");
            });

            // Register LLM client dynamically based on appsettings config
            var llmProvider = config["LLMProvider"]?.ToLowerInvariant();

            switch (llmProvider)
            {
                case "openai":
                    services.AddScoped<ILlmClient, OpenAiClient>();
                    break;

                case "local":
                    services.AddScoped<ILlmClient, LocalLlmClient>();
                    break;

                default:
                    throw new Exception("Invalid or missing 'LLMProvider' in appsettings.json. Use 'OpenAI' or 'Local'.");
            }

            return services;
        }

        /// <summary>
        /// Registers application-layer use cases.
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IGetReviewsUseCase, GetReviewsUseCase>();
            services.AddScoped<IApproveReviewUseCase, ApproveReviewUseCase>();
            return services;
        }

        /// <summary>
        /// Retry policy using exponential backoff (2s, 4s, 8s) for transient HTTP errors.
        /// </summary>
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