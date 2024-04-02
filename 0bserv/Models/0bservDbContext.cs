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
        //scaffolding fix
        public _0bservDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<_0bservDbContext> optionsBuilder = new();
            _ = optionsBuilder.UseSqlServer("Server=localhost;Database=0bserv;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new _0bservDbContext(optionsBuilder.Options, _configuration);
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

        public List<FeedContentModel> SearchFeedContents(string keyword, DateTime? startDate, DateTime? endDate)
        {
            // Esegui una query parametrizzata per la ricerca full-text
            var searchResults = FeedContents
                .Where(f =>
                    (string.IsNullOrEmpty(keyword) ||
                    EF.Functions.Like(f.Title, $"%{keyword}%") ||
                    EF.Functions.Like(f.Description, $"%{keyword}%") ||
                    EF.Functions.Like(f.Link, $"%{keyword}%") ||
                    EF.Functions.Like(f.Author, $"%{keyword}%"))
                    && (!startDate.HasValue || f.PublishDate >= startDate)
                    && (!endDate.HasValue || f.PublishDate <= endDate))
                .ToList();

            return searchResults;
        }

        public void EnsureDatabaseCreated()
        {
            _ = Database.EnsureCreated();
        }
    }
}

