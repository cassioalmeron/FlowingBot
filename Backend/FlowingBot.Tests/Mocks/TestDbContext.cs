using FlowingBot.Core;
using Microsoft.EntityFrameworkCore;

namespace FlowingBot.Tests.Mocks
{
    internal class TestDbContext : FlowingBotDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Here it configured the Memory Database
            optionsBuilder.UseInMemoryDatabase("Virtual-Database-Name");
        }
    }
}