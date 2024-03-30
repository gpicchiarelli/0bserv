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
        public DbSet<RssArticle> Articles { get; set; }
        public DbSet<WebSiteItem> Sites { get; set; }
        public DbSet<WebArticle> WArticles { get; set; }

    }

    public class WebSiteItem
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }

    public class RssFeed
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }

    public class WebArticle
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime PublishedAt { get; set; }
        public string Url { get; set; }
        public int WebsiteId { get; set; }
        public WebSiteItem Website { get; set; }
    }

    public class RssArticle
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