using System.Runtime.CompilerServices;
using FlowingBot.Core;
using FlowingBot.Core.Models;
using FlowingBot.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowingBot.Api.Filters;
using Serilog;

namespace FlowingBot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LoggingActionFilter))]
    public class ChatController : ControllerBase
    {
        private readonly FlowingBotDbContext _context;
        private readonly LlmService _llmService;

        public ChatController(FlowingBotDbContext context, LlmService llmService)
        {
            _context = context;
            _llmService = llmService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Chat>>> Get() =>
            await _context.Chats
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();

        [HttpGet("{id}/Messages")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages(int id)
        {
            var service = new ChatGetMessagesService(_context);
            return await service.Execute(id);
        }

        [HttpPost]
        public async Task<ActionResult<Chat>> Post(string userPrompt, string collection)
        {
            var service = new ChatCreateService(_context);
            var chat = await service.Execute(userPrompt, collection);
            return chat;
        }

        [HttpPost("{id}/Messages")]
        public async IAsyncEnumerable<string> PostMessages(int id, string userPrompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var service = new ChatAskQuestionService(_context, _llmService);
            await foreach (var str in service.Execute(id, userPrompt, cancellationToken))
                yield return str;
        }
    }
}