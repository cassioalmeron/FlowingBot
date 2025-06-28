using FlowingBot.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowingBot.Core.Services
{
    public class ConfigurationSaveService
    {
        public ConfigurationSaveService(FlowingBotDbContext context) =>
            _context = context;

        private readonly FlowingBotDbContext _context;

        public async Task Execute(string key, string value)
        {
            var configuration = await _context.Configurations
                .AsTracking()
                .SingleOrDefaultAsync(x => x.Key == key);
                
            if (configuration == null)
            {
                configuration = new Configuration { Key = key, Value = value };
                _context.Configurations.Add(configuration);
            }
            else
                configuration.Value = value;

            await _context.SaveChangesAsync();
        }
    }
}