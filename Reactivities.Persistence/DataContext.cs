using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reactivities.Domain;

namespace Reactivities.Persistence
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options, ILogger<DataContext> logger) : base(options)
        {
            try {
                this.Database.Migrate();
            } catch (Exception ex) {
                logger.LogError(ex, "An error occurred during migration");
            }
        }

        public DbSet<Value> Values { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Value>()
                .HasData(
                    new Value { Id = 1, Name = "Value 101" },
                    new Value { Id = 2, Name = "Value 102" },
                    new Value { Id = 3, Name = "Value 103" }
                );
        }
    }
}
