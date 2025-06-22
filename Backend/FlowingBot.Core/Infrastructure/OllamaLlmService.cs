using System.Runtime.CompilerServices;
using FlowingBot.Core.Models;
using FlowingBot.Core.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FlowingBot.Core.Infrastructure
{
    public class OllamaLlmService : ILlmService
    {
        private readonly IConfiguration _configuration;

        public OllamaLlmService(IConfiguration configuration) =>
            _configuration = configuration;

        public async IAsyncEnumerable<string> ProcessChat(IEnumerable<Services.ChatMessage> messages, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var builder = Host.CreateApplicationBuilder();
            var modelName = _configuration.GetValue<string>("Ollama:ModelName");
            if (string.IsNullOrEmpty(modelName))
                throw new ArgumentException("The model is not defined!");

            builder.Services.AddChatClient(new OllamaChatClient(new Uri("http://localhost:11434"), modelName));
            var app = builder.Build();
            var chatClient = app.Services.GetRequiredService<IChatClient>();

            var iaMessages = await LoadExistingChat(messages);

            await foreach (var item in chatClient.GetStreamingResponseAsync(iaMessages).WithCancellation(cancellationToken))
                yield return item.Text;
        }

        private static async Task<List<Microsoft.Extensions.AI.ChatMessage>> LoadExistingChat(IEnumerable<Services.ChatMessage> messages)
        {
            var res = new List<Microsoft.Extensions.AI.ChatMessage>();
            foreach (var chatMessage2 in messages)
            {
                ChatRole role;
                switch (chatMessage2.Role)
                {
                    case RoleEnum.Assistant:
                        role = ChatRole.Assistant; 
                        break;
                    case RoleEnum.System:
                        role = ChatRole.System;
                        break;
                    case RoleEnum.User:
                        role = ChatRole.User;
                        break;
                }

                var message = new Microsoft.Extensions.AI.ChatMessage(role, chatMessage2.Message);

                res.Add(message);
            }

            return res;
        }
    }
}