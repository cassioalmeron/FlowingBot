using System.Runtime.CompilerServices;
using FlowingBot.Core.Models;
using FlowingBot.Core.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FlowingBot.Core.Infrastructure
{
    public class OllamaLlmService : ILlmService
    {
        public OllamaLlmService(string modelName) =>
            _modelName = modelName;

        private readonly string _modelName;

        public async IAsyncEnumerable<string> ProcessChat(IEnumerable<Services.ChatMessage> messages, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var builder = Host.CreateApplicationBuilder();

            builder.Services.AddChatClient(new OllamaChatClient(new Uri("http://localhost:11434"), _modelName));
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