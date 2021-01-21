using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reactivities.Domain;

namespace Reactivities.Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options, ILogger<DataContext> logger) : base(options)
        {
            try
            {
                this.Database.Migrate();
                Seed.SeedData(this);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during migration");
            }
        }

        public DbSet<Activity> Activities { get; init; }
        public DbSet<UserActivity> UserActivities { get; init; }
        public DbSet<Photo> Photos { get; init; }
        public DbSet<Comment> Comments { get; init; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserActivity>(x => x.HasKey(userActivity => new { userActivity.AppUserId, userActivity.ActivityId }));

            builder.Entity<UserActivity>()
                .HasOne(userActivity => userActivity.AppUser)
                .WithMany(user => user.UserActivities)
                .HasForeignKey(userActivity => userActivity.AppUserId);

            builder.Entity<UserActivity>()
                .HasOne(userActivity => userActivity.Activity)
                .WithMany(user => user.UserActivities)
                .HasForeignKey(userActivity => userActivity.ActivityId);

            builder.Entity<AppUser>()
                .HasMany(left => left.Followers)
                .WithMany(right => right.Followings)
                .UsingEntity(join => join.ToTable("UserFollowing"));
        }
    }
}