using FlowingBot.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowingBot.Core.Services
{
    public class ConfigurationSaveServiceAlternative
    {
        public ConfigurationSaveServiceAlternative(FlowingBotDbContext context) =>
            _context = context;

        private readonly FlowingBotDbContext _context;

        public async Task Execute(string key, string value)
        {
            var configuration = await _context.Configurations
                .SingleOrDefaultAsync(x => x.Key == key);
                
            if (configuration == null)
            {
                // Create new configuration
                var newConfig = new Configuration { Key = key, Value = value };
                _context.Configurations.Add(newConfig);
            }
            else
            {
                // Update existing configuration using raw SQL or detached entity
                var existingConfig = new Configuration 
                { 
                    Id = configuration.Id, 
                    Key = key, 
                    Value = value 
                };
                
                _context.Configurations.Update(existingConfig);
            }

            await _context.SaveChangesAsync();
        }

        // Alternative method using raw SQL
        public async Task ExecuteWithSql(string key, string value)
        {
            var configuration = await _context.Configurations
                .SingleOrDefaultAsync(x => x.Key == key);
                
            if (configuration == null)
            {
                // Insert new
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO Configurations (Key, Value) VALUES ({0}, {1})", 
                    key, value);
            }
            else
            {
                // Update existing
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE Configurations SET Value = {0} WHERE Key = {1}", 
                    value, key);
            }
        }
    }
} 