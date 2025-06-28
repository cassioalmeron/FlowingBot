using Microsoft.AspNetCore.Mvc;
using FlowingBot.Api.Filters;
using FlowingBot.Core;
using FlowingBot.Core.Models;
using FlowingBot.Core.Services;

namespace FlowingBot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LoggingActionFilter))]
    public class ConfigurationsController : ControllerBase
    {
        public ConfigurationsController(FlowingBotDbContext context) =>
            _context = context;

        private readonly FlowingBotDbContext _context;

        [HttpGet]
        public async Task<List<Configuration>> Get()
        {
            var configurationGetService = new ConfigurationGetService(_context);
            var result = await configurationGetService
                .Execute();
            return result;
        }

        [HttpPost]
        public async Task Post(List<Configuration> configurations)
        {
            var configurationSaveService = new ConfigurationSaveService(_context);
            foreach (var configuration in configurations)
                await configurationSaveService
                    .Execute(configuration.Key, configuration.Value);
        }
    }
}