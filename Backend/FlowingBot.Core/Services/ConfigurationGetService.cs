using FlowingBot.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowingBot.Core.Services
{
    public class ConfigurationGetService
    {
        public ConfigurationGetService(FlowingBotDbContext context) =>
            _context = context;

        private readonly FlowingBotDbContext _context;

        public async Task<List<Configuration>> Execute(string? key = null)
        {
            var messages = _context.Configurations.AsQueryable();
            if (!string.IsNullOrWhiteSpace(key))
                messages = messages.Where(x => x.Key == key);

            var result = await messages
                .ToListAsync();

            return result;
        }

        // Synchronous version for use in constructors
        public string GetValueSync(string key)
        {
            var configuration = _context.Configurations
                .SingleOrDefault(x => x.Key == key);
                
            if (configuration == null)
                throw new Exception($"\"{key}\" not configured!");

            return configuration.Value;
        }
    }
}