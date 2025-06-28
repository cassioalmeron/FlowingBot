using FlowingBot.Core.Services;
using FlowingBot.Tests.Mocks;

namespace FlowingBot.Tests
{
    [TestClass]
    public class ConfigurationGetServiceTest
    {
        [TestMethod]
        public async Task ConfigurationGetService_GetValue()
        {
            var context = new TestDbContext();
            var service = new ConfigurationSaveService(context);
            await service.Execute("Key", "Value");

            var configurationService = new ConfigurationGetService(context);

            var value = configurationService.GetValueSync("Key");

            Assert.AreEqual("Value", value);
        }

        [TestMethod]
        public async Task ConfigurationGetService_GetValue_NotExists()
        {
            var context = new TestDbContext();
            var configurationService = new ConfigurationGetService(context);

            try
            {
                configurationService.GetValueSync("Key");
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("\"Key\" not configured!", e.Message);
            }
        }
    }
}