using FlowingBot.Core.Services;
using FlowingBot.Tests.Mocks;

namespace FlowingBot.Tests
{
    [TestClass]
    public class ChatCreateServiceTest
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var context = new TestDbContext();
            var service = new ChatCreateService(context);
            await service.Execute("Title", "CollectionTest");

            var chat = context.Chats.Single();

            Assert.AreEqual("Title", chat.Title);
        }
    }
}