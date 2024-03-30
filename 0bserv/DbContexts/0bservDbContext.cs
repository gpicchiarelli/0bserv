using System.Reflection;
using _0bserv.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace _0bserv.DbContexts
{
    public class _0bservDbContext : DbContext
    {
        public DbSet<RssFeed> RssFeeds { get; set; }
        public DbSet<FeedContent> FeedContents { get; set; }

        public _0bservDbContext(DbContextOptions<_0bservDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information); // Abilita il logging delle query su Console
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            _ = modelBuilder.Entity<RssFeed>()
                .HasKey(r => r.Id);
            _ = modelBuilder.Entity<FeedContent>()
                .HasKey(f => f.Id);
            _ = modelBuilder.Entity<FeedContent>()
                .HasOne(f => f.RssFeed)
                .WithMany(r => r.Contents)
                .HasForeignKey(f => f.RssFeedId);
            _ = modelBuilder.Entity<RssFeed>()
                .Property(r => r.Url)
                .IsRequired()
                .HasMaxLength(255);
            _ = modelBuilder.Entity<FeedContent>()
                .HasIndex(f => f.PublishDate);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}

