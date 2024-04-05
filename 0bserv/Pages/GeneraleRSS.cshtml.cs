using System.ServiceModel.Syndication;
using System.Xml;
using _0bserv.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
            HttpRequest request = _httpContextAccessor.HttpContext.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";

            // Costruisci l'URL completo dell'endpoint attuale
            string currentUrl = $"{baseUrl}{request.Path}";

            // Utilizza l'URL completo come URI per il feed RSS
            SyndicationFeed feed = new("Feed RSS 0bserv", "Feed RSS 0bserv", new Uri(currentUrl));
            List<FeedContentModel> feedContents = await _context.FeedContents.OrderByDescending(f => f.PublishDate).ToListAsync(); // Assicurati di utilizzare ToListAsync() per operazioni asincrone
            List<SyndicationItem> items = new();

            // Creazione degli elementi del feed RSS
            foreach (FeedContentModel? feedContent in feedContents)
            {
                // Creazione di un nuovo item del feed RSS
                SyndicationItem item = new()
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
            MemoryStream response = new(); // Utilizza una MemoryStream per memorizzare il contenuto XML
            using (XmlWriter writer = XmlWriter.Create(response, new XmlWriterSettings { Async = true }))
            {
                Rss20FeedFormatter rssFormatter = new(feed);
                rssFormatter.WriteTo(writer);
                await writer.FlushAsync(); // Assicurati che tutti i dati vengano scritti in modo asincrono
            }
            _ = response.Seek(0, SeekOrigin.Begin); // Assicurati che la posizione del flusso sia impostata all'inizio
            return File(response, "application/rss+xml"); // Restituisci il contenuto XML come file
        }

    }
}