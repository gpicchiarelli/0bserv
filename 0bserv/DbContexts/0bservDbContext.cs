using System.Configuration;
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
        private readonly IConfiguration _configuration;

        public _0bservDbContext(DbContextOptions<_0bservDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
        //scaffolding fix
        public _0bservDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<_0bservDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=0bserv;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new _0bservDbContext(optionsBuilder.Options,_configuration);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("MSSQL");
            optionsBuilder.UseSqlServer(connectionString);
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information); // Abilita il logging delle query su Console
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aggiungi DbSets programmaticamente
            modelBuilder.Entity<RssFeed>().ToTable("RssFeeds");
            modelBuilder.Entity<FeedContent>().ToTable("FeedContents");

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
            _=  modelBuilder.Entity<RssFeed>()
                .HasIndex(feed => feed.Url)
                .IsUnique();
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public void EnsureDatabaseCreated()
        {
            Database.EnsureCreated();
        }
    }
}

