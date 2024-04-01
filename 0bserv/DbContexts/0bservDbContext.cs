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

        public string GetConnectionString()
        {
            // retrieve App Service connection string
            var myConnString = _configuration.GetConnectionString("MSSQL");
            return myConnString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aggiungi DbSets programmaticamente
            modelBuilder.Entity<FeedModel>().ToTable("RssFeeds");
            modelBuilder.Entity<FeedContentModel>().ToTable("FeedContents");

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
            _=  modelBuilder.Entity<FeedModel>()
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

