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
            services.AddScoped<IBusinessRepository, BusinessRepository>();
            services.AddScoped<IOAuthTokenRepository, OAuthTokenRepository>();
            services.AddScoped<IGoogleOAuthService, GoogleOAuthService>();
            services.AddScoped<IBusinessContextService, BusinessContextService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<AutoReplyService>();
            services.AddScoped<SentimentService>();
            services.AddScoped<IReviewApprovalService, ReviewApprovalService>();
            services.AddScoped<IGoogleReviewIngestionService, GoogleReviewIngestionService>();
            services.AddScoped<IReviewPollingService, ReviewPollingService>();
            services.AddScoped<GoogleReviewClient>();
            services.AddScoped<AuthenticatedGoogleApiClient>();
            
            // HTTP context for business context service
            services.AddHttpContextAccessor();

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
            // Register the active LLM implementation and wrap with retry/fallback handler
            services.AddScoped<ILlmClient>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                var logger = sp.GetRequiredService<ILogger<ResilientLlmClient>>();
                var provider = config["LLMProvider"]?.ToLowerInvariant();

                ILlmClient baseClient = provider switch
                {
                    "openai" => new OpenAiClient(factory, config),
                    "local" => new LocalLlmClient(factory, config),
                    _ => throw new InvalidOperationException("Unsupported LLM provider configured.")
                };

                return new ResilientLlmClient(baseClient, logger);
            });

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