using System.Runtime.CompilerServices;
using System.Text;
using FlowingBot.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowingBot.Core.Services
{
    public class ChatAskQuestionService
    {
        private readonly FlowingBotDbContext _context;
        private readonly LlmService _llmService;

        public ChatAskQuestionService(
            FlowingBotDbContext context, LlmService llmService)
        {
            _context = context;
            _llmService = llmService;
        }

        public async IAsyncEnumerable<string> Execute(int chatId, string userPrompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var userMessage = new Message
            {
                ChatId = chatId,
                Role = RoleEnum.User,
                Text = userPrompt
            };
            _context.Messages.Add(userMessage);

            var assistantMessage = new Message
            {
                ChatId = chatId,
                Role = RoleEnum.Assistant,
                Text = string.Empty
            };
            _context.Messages.Add(assistantMessage);

            var chat = await _context.Chats
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == chatId, cancellationToken: cancellationToken);

            var sb = new StringBuilder();
            await foreach (var text in _llmService.ProcessChat(chat, userPrompt, cancellationToken))
            {
                sb.Append(text);
                yield return text;
            }

            assistantMessage.Text = sb.ToString();

            _context.SaveChangesAsync();
        }
    }
}