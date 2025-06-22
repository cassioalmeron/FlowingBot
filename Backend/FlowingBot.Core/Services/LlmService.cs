using System.Runtime.CompilerServices;
using FlowingBot.Core.Models;

namespace FlowingBot.Core.Services
{
    public interface ILlmService
    {
        IAsyncEnumerable<string> ProcessChat(IEnumerable<ChatMessage> messages,
            [EnumeratorCancellation] CancellationToken cancellationToken = default);
    }

    public record ChatMessage(RoleEnum Role, string Message);

    public class LlmService
    {
        private readonly ILlmService _llmService;
        private readonly VectorDatabaseService _vectorDatabaseService;

        public LlmService(VectorDatabaseService vectorDatabaseService, ILlmService llmService)
        {
            _llmService = llmService;
            _vectorDatabaseService = vectorDatabaseService;
        }

        public async IAsyncEnumerable<string> ProcessChat(Chat chat, string userPrompt,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var chatHistory = await LoadExistingChat(chat, userPrompt);

            await foreach (var chunk in _llmService.ProcessChat(chatHistory).WithCancellation(cancellationToken))
                yield return chunk;
        }

        private async Task<List<ChatMessage>> LoadExistingChat(Chat chat, string userPrompt)
        {
            var assistantText = await GenerateAssistantText(chat, userPrompt);

            var res = new List<ChatMessage> { new(RoleEnum.System, assistantText) };

            res.AddRange(chat.Messages.Select(message => new ChatMessage(
                message.Role == RoleEnum.Assistant ? RoleEnum.Assistant : RoleEnum.User,
                message.Text)));

            res.Add(new ChatMessage(RoleEnum.User, userPrompt));

            return res;
        }

        private async Task<string> GenerateAssistantText(Chat chat, string userPrompt)
        {
            var assistantText = chat.AssistantText;
            if (string.IsNullOrEmpty(assistantText))
                assistantText = "You are an assistant for question-answering tasks. \n" +
                                "Use the following pieces of retrieved context to answer the question, and nothing else. \n" +
                                //"The relevance of the content is ordered, the first content. \n" +
                                "If you don't know the answer, say that you don't know. \n" +
                                "Use three sentences maximum and keep the answer concise. \n\n";

            var faqAnswer = await _vectorDatabaseService.QueryAsync(chat.CollectionName, userPrompt);

            assistantText += $"Context:\n {string.Join("\n\n", faqAnswer)}";

            return assistantText;
        }
    }
}