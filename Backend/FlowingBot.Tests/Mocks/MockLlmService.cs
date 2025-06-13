using FlowingBot.Core.Infrastructure;
using FlowingBot.Core.Models;

namespace FlowingBot.Tests.Mocks
{
    public class MockLlmService : ILlmService
    {
        private readonly string[] _mockResponses;

        public MockLlmService(params string[] mockResponses)
        {
            _mockResponses = mockResponses;
        }

        public async IAsyncEnumerable<string> ProcessChat(Chat chat, string userPrompt, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var response in _mockResponses)
            {
                yield return response;
                await Task.Delay(100, cancellationToken); // Simulate some delay like a real LLM
            }
        }
    }
} 