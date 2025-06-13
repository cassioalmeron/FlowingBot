using FlowingBot.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowingBot.Core.Services
{
    public class ChatGetMessagesService
    {
        public ChatGetMessagesService(FlowingBotDbContext context) =>
            _context = context;

        private readonly FlowingBotDbContext _context;

        public async Task<List<Message>> Execute(int id)
        {
            var messages = await _context.Messages
                .Where(x => x.ChatId == id)
                .OrderBy(x => x.Timestamp)
                .ToListAsync();
            return messages;
        }
    }
}