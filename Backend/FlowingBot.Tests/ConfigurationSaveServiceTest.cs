using FlowingBot.Core.Services;
using FlowingBot.Tests.Mocks;

namespace FlowingBot.Tests
{
    [TestClass]
    public class ConfigurationSaveServiceTest
    {
        [TestMethod]
        public async Task ConfigurationSaveService_Execute()
        {
            var context = new TestDbContext();
            var service = new ConfigurationSaveService(context);
            await service.Execute("Title", "CollectionTest");

            var configuration = context.Configurations.Single();

            Assert.AreEqual("Title", configuration.Key);
            Assert.AreEqual("CollectionTest", configuration.Value);
        }

        [TestMethod]
        public async Task ConfigurationSaveService_Execute_Twice()
        {
            var context = new TestDbContext();
            var service = new ConfigurationSaveService(context);
            await service.Execute("Title", "CollectionTest");
            await service.Execute("Key", "Value");

            Assert.AreEqual(2, context.Configurations.Count());

            var configuration = context.Configurations.First();

            Assert.AreEqual("Title", configuration.Key);
            Assert.AreEqual("CollectionTest", configuration.Value);

            configuration = context.Configurations.Last();

            Assert.AreEqual("Key", configuration.Key);
            Assert.AreEqual("Value", configuration.Value);
        }

        [TestMethod]
        public async Task ConfigurationSaveService_Execute_UpdateExisting()
        {
            var context = new TestDbContext();
            var service = new ConfigurationSaveService(context);
            await service.Execute("Key", "Value");
            await service.Execute("Key", "Value 2");

            Assert.AreEqual(1, context.Configurations.Count());

            var configuration = context.Configurations.Single();

            Assert.AreEqual("Key", configuration.Key);
            Assert.AreEqual("Value 2", configuration.Value);
        }
    }
}