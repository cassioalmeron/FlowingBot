using FlowingBot.Core.Infrastructure;
using FlowingBot.Core.Models;
using FlowingBot.Core.Services;
using FlowingBot.Tests.Mocks;

namespace FlowingBot.Tests
{
    [TestClass]
    public class ChatAskQuestionServiceTest
    {
        [TestMethod]
        public async Task ChatAskQuestionService_Execute()
        {
            // Arrange
            var context = new TestDbContext();
            var chat = new Chat { Id = 1, Title = "Test Chat", AssistantText = "Test Assistant Text"};
            context.Chats.Add(chat);
            await context.SaveChangesAsync();

            var mockResponses = new[] { "Hello", "How can I help you?" };
            var mockLlmService = new MockLlmService(mockResponses);
            var mockVectorDatabaseService = new MockVectorDatabaseService();
            var vectorDatabaseService = new VectorDatabaseService(mockVectorDatabaseService);
            var llmService = new LlmService(vectorDatabaseService, mockLlmService);
            var service = new ChatAskQuestionService(context, llmService);

            // Act
            var responses = new List<string>();
            await foreach (var response in service.Execute(1, "My Prompt"))
                responses.Add(response);

            // Assert
            Assert.AreEqual(2, responses.Count);
            Assert.AreEqual("Hello", responses[0]);
            Assert.AreEqual("How can I help you?", responses[1]);

            var messages = context.Messages.Where(m => m.ChatId == 1).ToList();
            Assert.AreEqual(2, messages.Count); // One user message and one assistant message
            Assert.AreEqual("My Prompt", messages[0].Text);
            Assert.AreEqual("HelloHow can I help you?", messages[1].Text);
        }
    }
}