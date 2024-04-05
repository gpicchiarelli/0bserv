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
using Microsoft.EntityFrameworkCore;

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
            var feed = new SyndicationFeed("Feed RSS 0bserv", "Feed RSS 0bserv", new Uri(currentUrl));
            var feedContents = await _context.FeedContents.OrderByDescending(f => f.PublishDate).ToListAsync(); // Assicurati di utilizzare ToListAsync() per operazioni asincrone
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
            var response = new MemoryStream(); // Utilizza una MemoryStream per memorizzare il contenuto XML
            using (var writer = XmlWriter.Create(response, new XmlWriterSettings { Async = true }))
            {
                var rssFormatter = new Rss20FeedFormatter(feed);
                rssFormatter.WriteTo(writer);
                await writer.FlushAsync(); // Assicurati che tutti i dati vengano scritti in modo asincrono
            }
            response.Seek(0, SeekOrigin.Begin); // Assicurati che la posizione del flusso sia impostata all'inizio
            return File(response, "application/rss+xml"); // Restituisci il contenuto XML come file
        }

    }
}