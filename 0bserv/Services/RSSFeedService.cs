using Observ.Models;
using System.Xml;
using Microsoft.SyndicationFeed;

namespace _0bserv.Services
{
    public class NewsAggregatorService
    {
        private readonly ApplicationDbContext _context;

        public NewsAggregatorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AggregateNewsFromRssFeeds(IEnumerable<RssFeed> rssFeeds)
        {
            foreach (var feed in rssFeeds)
            {
                var syndicationFeed = await GetSyndicationFeed(feed.Url);

                foreach (var item in syndicationFeed.Items)
                {
                    var article = new Article
                    {
                        Title = item.Title.Text,
                        Description = item.Summary.Text,
                        Link = item.Links.FirstOrDefault()?.Uri.AbsoluteUri,
                        Author = item.Authors.FirstOrDefault()?.Name,
                        PublishDate = item.PublishDate.DateTime,
                        RssFeedId = feed.Id
                    };

                    _context.Articles.Add(article);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task<SyndicationFeed> GetSyndicationFeed(string url)
        {
            using (var reader = XmlReader.Create(url))
            {
                return await Task.Run(() => SyndicationFeed.Load(reader));
            }
        }
    }

    public class RSSFeedBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public RSSFeedBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var newsAggregatorService = scope.ServiceProvider.GetRequiredService<NewsAggregatorService>();
                    await newsAggregatorService.AggregateNewsFromRssFeeds();
                }
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
