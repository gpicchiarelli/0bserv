using System;
using System.ServiceModel.Syndication;
using System.Web;
using _0bserv.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace _0bserv.Pages
{
    public class RssFeedProviderModel : PageModel
    {
        private readonly ILogger<RssFeedProviderModel> _logger;
        private readonly _0bservDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RssFeedProviderModel(ILogger<RssFeedProviderModel> logger, _0bservDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> OnGet()
        {

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            // Costruisci l'URL completo dell'endpoint attuale
            var currentUrl = $"{baseUrl}{request.Path}";

            // Utilizza l'URL completo come URI per il feed RSS
            var feed = new SyndicationFeed("Feed RSS 0bserv", "Feed RSS 0bserv", new Uri(currentUrl)); var feedContents = _context.FeedContents.ToList(); 
            var items = new List<SyndicationItem>();

            // Creazione degli elementi del feed RSS
            foreach (var feedContent in feedContents)
            {
                // Creazione di un nuovo item del feed RSS
                var item = new SyndicationItem
                {
                    Title = new TextSyndicationContent(feedContent.Title),
                    Content = new TextSyndicationContent(feedContent.Description),
                    PublishDate = new DateTimeOffset(feedContent.PublishDate),
                    Id = feedContent.Id.ToString() // Opzionale: imposta un ID univoco per l'elemento del feed
                };

                // Aggiunta del link all'elemento del feed RSS
                if (!string.IsNullOrEmpty(feedContent.Link))
                {
                    item.Links.Add(new SyndicationLink(new Uri(feedContent.Link)));
                }
                items.Add(item);
            }

            feed.Items = items;
            // Scrivi il feed RSS come risposta HTTP
            var response = HttpContext.Response;
            response.ContentType = "application/rss+xml";

            using (var writer = XmlWriter.Create(response.Body, new XmlWriterSettings { Async = true }))
            {
                var rssFormatter = new Rss20FeedFormatter(feed);
                rssFormatter.WriteTo(writer);
            }
            return new ContentResult
            {
                ContentType = "application/rss+xml",
                Content = response.Body.ToString(),
            };
        }
    }
}