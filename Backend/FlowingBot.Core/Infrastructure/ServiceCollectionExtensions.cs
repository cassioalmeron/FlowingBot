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
            services.AddScoped<ConfigurationGetService>();
            services.AddScoped<ConfigurationSaveService>();
            //services.AddScoped<ConfigurationSeedService>();

            // Register the factory
            services.AddScoped<LlmServiceFactory>();

            // Register ILlmService using factory pattern
            services.AddScoped<ILlmService>(provider =>
            {
                var factory = provider.GetRequiredService<LlmServiceFactory>();
                return factory.CreateLlmService();
            });

            services.AddScoped<IVectorDatabaseService, QdrantService>();

            return services;
        }
    }
}