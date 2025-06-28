using FlowingBot.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FlowingBot.Core.Infrastructure
{
    public class LlmServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConfigurationGetService _configurationService;
        private readonly string _modelName;

        public LlmServiceFactory(IServiceProvider serviceProvider, ConfigurationGetService configurationService)
        {
            _serviceProvider = serviceProvider;
            _configurationService = configurationService;

            _modelName = _configurationService.GetValueSync("ModelName");
        }

        public ILlmService CreateLlmService()
        {
            var configurationService = _serviceProvider.GetRequiredService<ConfigurationGetService>();
            var source = configurationService.GetValueSync("Source");

            switch (source)
            {
                case "Ollama":
                    return CreateOllamaService();
                case "OpenAI":
                    return CreateOpenAiService();
                default:
                    throw new Exception($"Invalid source: {source}");
            }
        }

        private ILlmService CreateOllamaService() =>
            new OllamaLlmService(_modelName);

        private ILlmService CreateOpenAiService()
        {
            var apiKey = _configurationService.GetValueSync("OpenAIKey");
            return new OpenAiLlmService(_modelName, apiKey);
        }
    }
}