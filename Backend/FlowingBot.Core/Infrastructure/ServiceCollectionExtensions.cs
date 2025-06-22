using FlowingBot.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FlowingBot.Core.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructuralServices(this IServiceCollection services)
        {
            services.AddScoped<LlmService>();
            services.AddScoped<VectorDatabaseService>();


            //services.AddScoped<ILlmService, OllamaLlmService>();
            services.AddScoped<ILlmService, OpenAiLlmService>();

            services.AddScoped<IVectorDatabaseService, QdrantService>();

            return services;
        }
    }
}