using FlowingBot.Core.Models;
using FlowingBot.Core.Services;
using FlowingBot.Tests.Mocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public async Task ConfigurationSaveService_Execute_CreateNew()
        {
            var context = new TestDbContext();
            var service = new ConfigurationSaveService(context);
            await service.Execute("Key", "Value");

            Assert.AreEqual(1, context.Configurations.Count());

            var configuration = context.Configurations.Single();

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

        [TestMethod]
        public async Task ConfigurationSaveService_Execute_UpdateExistingWithNoTracking()
        {
            var context = new TestDbContext();
            var service = new ConfigurationSaveService(context);
            
            // Create initial configuration
            await service.Execute("Key", "Value");
            
            // Verify it was created
            var initialConfig = context.Configurations.Single();
            Assert.AreEqual("Value", initialConfig.Value);
            
            // Update the configuration
            await service.Execute("Key", "Value 2");
            
            // Reload from database to verify the update was persisted
            context.ChangeTracker.Clear(); // Clear the change tracker
            var updatedConfig = context.Configurations.Single();
            
            Assert.AreEqual("Key", updatedConfig.Key);
            Assert.AreEqual("Value 2", updatedConfig.Value);
        }

        [TestMethod]
        public async Task ConfigurationSaveService_Execute_MultipleUpdates()
        {
            var context = new TestDbContext();
            var service = new ConfigurationSaveService(context);
            
            // Create and update multiple times
            await service.Execute("Key1", "Value1");
            await service.Execute("Key2", "Value2");
            await service.Execute("Key1", "Value1Updated");
            await service.Execute("Key2", "Value2Updated");
            
            // Clear change tracker and reload
            context.ChangeTracker.Clear();
            
            var configurations = context.Configurations.ToList();
            
            Assert.AreEqual(2, configurations.Count);
            
            var key1Config = configurations.First(c => c.Key == "Key1");
            var key2Config = configurations.First(c => c.Key == "Key2");
            
            Assert.AreEqual("Value1Updated", key1Config.Value);
            Assert.AreEqual("Value2Updated", key2Config.Value);
        }

        [TestMethod]
        public async Task ConfigurationSaveService_Execute_NoEntityTrackingConflict()
        {
            var context = new TestDbContext();
            var service = new ConfigurationSaveService(context);
            
            // This should not throw an exception about entity tracking conflicts
            await service.Execute("Key", "Value");
            await service.Execute("Key", "Value2");
            await service.Execute("Key", "Value3");
            
            // Verify the final value
            context.ChangeTracker.Clear();
            var finalConfig = context.Configurations.Single();
            Assert.AreEqual("Value3", finalConfig.Value);
        }
    }
}