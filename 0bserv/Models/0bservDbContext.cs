using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace _0bserv.Models
{
    public class _0bservDbContext : DbContext
    {
        public DbSet<FeedModel> RssFeeds { get; set; }
        public DbSet<FeedContentModel> FeedContents { get; set; }
        private readonly IConfiguration _configuration;

        public _0bservDbContext(DbContextOptions<_0bservDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = _configuration.GetConnectionString("MSSQL");
            _ = optionsBuilder.UseSqlServer(connectionString);
            _ = optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information); // Abilita il logging delle query su Console
        }

        public string GetConnectionString()
        {
            // retrieve App Service connection string
            string? myConnString = _configuration.GetConnectionString("MSSQL");
            return myConnString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aggiungi DbSets programmaticamente
            _ = modelBuilder.Entity<FeedModel>().ToTable("RssFeeds");
            _ = modelBuilder.Entity<FeedContentModel>().ToTable("FeedContents");

            _ = modelBuilder.Entity<FeedModel>()
                .HasKey(r => r.Id);
            _ = modelBuilder.Entity<FeedContentModel>()
                .HasKey(f => f.Id);
            _ = modelBuilder.Entity<FeedContentModel>()
                .HasOne(f => f.RssFeed)
                .WithMany(r => r.Contents)
                .HasForeignKey(f => f.RssFeedId);
            _ = modelBuilder.Entity<FeedModel>()
                .Property(r => r.Url)
                .IsRequired()
                .HasMaxLength(255);
            _ = modelBuilder.Entity<FeedContentModel>()
                .HasIndex(f => f.PublishDate);
            _ = modelBuilder.Entity<FeedModel>()
                .HasIndex(feed => feed.Url)
                .IsUnique();
            _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }

        public void EnsureDatabaseCreated()
        {
            _ = Database.EnsureCreated();
        }
    }
}

