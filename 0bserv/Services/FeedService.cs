﻿using System.ServiceModel.Syndication;
using System.Xml;
using _0bserv.Models;
using Microsoft.EntityFrameworkCore;

namespace _0bserv.Services
{
    public class FeedService : BackgroundService
    {
        private readonly ILogger<FeedService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public FeedService(ILogger<FeedService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("FeedService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Sto controllando i feed RSS...");

                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    _0bservDbContext dbContext = scope.ServiceProvider.GetRequiredService<_0bservDbContext>();
                    List<FeedModel> feeds = await dbContext.RssFeeds.ToListAsync(stoppingToken);
                    IEnumerable<Task> tasks = feeds.Select(feed => ProcessFeed(feed, dbContext, stoppingToken));
                    await Task.WhenAll(tasks);
                    _logger.LogInformation("Termine del controllo dei feed RSS.");

                    // Effettua il "pruning" dei feed più vecchi di 12 mesi
                    await PruneOldFeeds(dbContext);
                }

                // Attendere 5 minuti prima di controllare nuovamente i feed
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            _logger.LogInformation("FeedService is stopping.");
        }

        private async Task ProcessFeed(FeedModel feed, _0bservDbContext dbContext, CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation($"Processing feed: {feed.Url}");

                using (var xmlReader = XmlReader.Create(feed.Url))
                {
                    var syndicationFeed = SyndicationFeed.Load(xmlReader);
                    if (syndicationFeed != null)
                    {
                        foreach (var item in syndicationFeed.Items)
                        {
                            var feedLink = item.Links.FirstOrDefault()?.Uri?.AbsoluteUri;

                            // Ottenere l'autore e la data di pubblicazione in modo sicuro
                            var autore = item.Authors.FirstOrDefault()?.Name ?? feed.Url;
                            var dataPubblicazione = item.PublishDate.DateTime != default ? item.PublishDate.DateTime : DateTime.Now;

                            // Controlla se l'elemento del feed è già stato memorizzato
                            var existingFeedContent = await dbContext.FeedContents.FirstOrDefaultAsync(fc => fc.Link == feedLink);

                            if (existingFeedContent == null)
                            {
                                // Se non esiste, memorizza il nuovo contenuto
                                var newFeedContent = new FeedContentModel
                                {
                                    RssFeed = feed,
                                    Title = item.Title.Text,
                                    Description = item.Summary.Text,
                                    Link = feedLink,
                                    Author = autore,
                                    PublishDate = dataPubblicazione
                                };

                                dbContext.FeedContents.Add(newFeedContent);
                                _logger.LogInformation($"Aggiunto contenuto: {newFeedContent.Title}");
                            }
                            else
                            {
                                _logger.LogInformation($"Contenuto già esistente: {existingFeedContent.Title}");
                            }
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing feed: {feed.Url}. Error: {ex.Message}");
            }
        }

        private async Task PruneOldFeeds(_0bservDbContext dbContext)
        {
            DateTime thresholdDate = DateTime.UtcNow.AddMonths(-12);

            List<FeedContentModel> oldFeedContents = await dbContext.FeedContents.Where(fc => fc.PublishDate < thresholdDate).ToListAsync();
            dbContext.FeedContents.RemoveRange(oldFeedContents);
            _ = await dbContext.SaveChangesAsync();
            _logger.LogInformation($"Pruned {oldFeedContents.Count} vecchi contenuti.");
        }
    }
}
