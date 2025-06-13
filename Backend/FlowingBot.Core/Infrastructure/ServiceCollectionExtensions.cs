using Microsoft.Extensions.DependencyInjection;

namespace FlowingBot.Core.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructuralServices(this IServiceCollection services)
        {
            services.AddScoped<ILlmService, LlmService>();
            return services;
        }
    }
} 