using Athos.ReviewAutomation.Core.Interfaces;
using Athos.ReviewAutomation.Infrastructure.LLM;
using Athos.ReviewAutomation.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Athos.ReviewAutomation.Application.AI;

public static class LlmRegistrationExtensions
{
    public static IServiceCollection AddLlmClient(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["LLMProvider"]?.ToLowerInvariant();

        return provider switch
        {
            "openai" => services.AddScoped<ILlmClient, OpenAiClient>(),
            "local" => services.AddScoped<ILlmClient, LocalLlmClient>(),
            _ => throw new InvalidOperationException($"Unsupported LLMProvider: '{provider}' in config")
        };
    }
}