using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Observ.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RssFeed> RssFeeds { get; set; }
        public DbSet<Article> Articles { get; set; }
    }

    public class RssFeed
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }

    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Author { get; set; }
        public DateTime PublishDate { get; set; }

        public int RssFeedId { get; set; }
        public RssFeed RssFeed { get; set; }
    }
}