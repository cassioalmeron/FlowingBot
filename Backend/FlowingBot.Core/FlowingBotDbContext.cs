﻿using System.Reflection;
using FlowingBot.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowingBot.Core
{
    public class FlowingBotDbContext : DbContext
    {
        public FlowingBotDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DBPath = Path.Combine(path, "FlowingBot.db");
        }

        public string DBPath { get; }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Configuration> Configurations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            =>
                optionsBuilder.UseSqlite($"Data Source={DBPath}")
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) // Default not tracking the changes in the objects generated by EF.
                    //.UseLazyLoadingProxies() // Configure Lazy Loads
                    .LogTo(Console.WriteLine)
                    // The two bellow, do not allow in production, just for debugging and educational purposes
                    .EnableSensitiveDataLogging() 
                    .EnableDetailedErrors();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}