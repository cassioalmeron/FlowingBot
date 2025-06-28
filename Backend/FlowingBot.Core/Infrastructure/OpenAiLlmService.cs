using System.ClientModel;
using System.Runtime.CompilerServices;
using FlowingBot.Core.Models;
using FlowingBot.Core.Services;
using OpenAI;
using OpenAI.Chat;
using ChatMessage = OpenAI.Chat.ChatMessage;

namespace FlowingBot.Core.Infrastructure
{
    public class OpenAiLlmService : ILlmService
    {
        private readonly OpenAIClient _client;
        private readonly string _modelName;

        public OpenAiLlmService(string modelName, string apiKey)
        {
            _client = new OpenAIClient(new ApiKeyCredential(apiKey));
            _modelName = modelName;
        }

        private static List<ChatMessage> ConvertToOpenAIMessages(IEnumerable<Services.ChatMessage> messages)
        {
            var openAIMessages = new List<ChatMessage>();
            
            foreach (var message in messages)
            {
                switch (message.Role)
                {
                    case RoleEnum.System:
                        openAIMessages.Add(new SystemChatMessage(message.Message));
                        break;
                    case RoleEnum.User:
                        openAIMessages.Add(new UserChatMessage(message.Message));
                        break;
                    case RoleEnum.Assistant:
                        openAIMessages.Add(new AssistantChatMessage(message.Message));
                        break;
                }
            }
            
            return openAIMessages;
        }
        
        public async IAsyncEnumerable<string> ProcessChat(IEnumerable<Services.ChatMessage> messages,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var openAIMessages = ConvertToOpenAIMessages(messages);
            var chatClient = _client.GetChatClient(_modelName);

            await foreach (var update in chatClient.CompleteChatStreamingAsync(openAIMessages, cancellationToken: cancellationToken))
            {
                foreach (var part in update.ContentUpdate)
                {
                    if (cancellationToken.IsCancellationRequested)
                        yield break;

                    yield return part.Text;
                }
            }
        }
    }
}