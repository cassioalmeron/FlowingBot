using System.Security.Cryptography.X509Certificates;
using FlowingBot.Core;
using Microsoft.EntityFrameworkCore;

namespace FlowingBot.Tests.Mocks
{
    internal class TestDbContext : FlowingBotDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var rnd = new Random();

            // Here it configured the Memory Database
            optionsBuilder.UseInMemoryDatabase($"Virtual-Database-Name-{rnd.Next(1, 1000)}");
        }
    }
}