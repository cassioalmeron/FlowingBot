using FlowingBot.Core.Models;

namespace FlowingBot.Core.Services
{
    public class ChatCreateService
    {
        public ChatCreateService(FlowingBotDbContext context) =>
            _context = context;

        private readonly FlowingBotDbContext _context;

        public async Task<Chat> Execute(string userPrompt, string collectionName)
        {
            var chat = new Chat
            {
                Title = userPrompt,
                AssistantText = string.Empty,
                CollectionName = collectionName
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return chat;
        }
    }
}