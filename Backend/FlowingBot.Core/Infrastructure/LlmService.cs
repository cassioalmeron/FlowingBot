using System.Runtime.CompilerServices;
using FlowingBot.Core.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FlowingBot.Core.Infrastructure
{
    public interface ILlmService
    {
        IAsyncEnumerable<string> ProcessChat(Chat chat, string userPrompt,
            [EnumeratorCancellation] CancellationToken cancellationToken = default);
    }

    public class LlmService : ILlmService
    {
        private readonly IConfiguration _configuration;

        public LlmService(IConfiguration configuration) =>
            _configuration = configuration;

        public async IAsyncEnumerable<string> ProcessChat(Chat chat, string userPrompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var chatHistory = await LoadExistingChat(chat, userPrompt);

            var builder = Host.CreateApplicationBuilder();
            var modelName = _configuration.GetValue<string>("Ollama:ModelName");
            if (string.IsNullOrEmpty(modelName))
                throw new ArgumentException("The model is not defined!");

            builder.Services.AddChatClient(new OllamaChatClient(new Uri("http://localhost:11434"), modelName));
            var app = builder.Build();
            var chatClient = app.Services.GetRequiredService<IChatClient>();

            await foreach (var item in chatClient.GetStreamingResponseAsync(chatHistory).WithCancellation(cancellationToken))
                yield return item.Text;
        }

        private static async Task<List<ChatMessage>> LoadExistingChat(Chat chat, string userPrompt)
        {
            var assistantText = await GenerateAssistantText(chat, userPrompt);

            var res = new List<ChatMessage> { new(ChatRole.System, assistantText) };

            res.AddRange(chat.Messages.Select(message => new ChatMessage(
                message.Role == RoleEnum.Assistant ? ChatRole.Assistant : ChatRole.User,
                message.Text)));

            res.Add(new ChatMessage(ChatRole.User, userPrompt));

            return res;
        }

        private static async Task<string> GenerateAssistantText(Chat chat, string userPrompt)
        {
            var assistantText = chat.AssistantText;
            if (string.IsNullOrEmpty(assistantText))
                assistantText = "You are an assistant for question-answering tasks. \n" +
                                "Use the following pieces of retrieved context to answer the question, and nothing else. \n" +
                                //"The relevance of the content is ordered, the first content. \n" +
                                "If you don't know the answer, say that you don't know. \n" +
                                "Use three sentences maximum and keep the answer concise. \n\n";

            var faqAnswer = await FindBestMatch(userPrompt, chat.CollectionName);

            assistantText += $"Context:\n {string.Join("\n\n", faqAnswer)}";

            return assistantText;
        }

        private static async Task<string[]> FindBestMatch(string userQuestion, string collectionName)
        {
            var service = new QdrantService(collectionName);
            var results = await service.QueryAsync(userQuestion);
            return results;
        }
    }
}