using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _0bserv.DbContexts;
using _0bserv.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Linq;
using System.Security.Policy;
using System.Drawing.Printing;

namespace _0bserv.Pages
{
    public class RssFeedModel : PageModel
    {
        public string ErrorMessage { get; set; }
        public List<RssFeed> FeedList { get; set; }
        private readonly _0bservDbContext _dbContext;
        public int PageIndex = 1;
        public int TotalPages = 1;
        public string? Url { get; set; }
        private const int DefaultPageSize = 50; // Numero predefinito di elementi per pagina


        public RssFeedModel(_0bservDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task OnPostAsync()
        {
            string _url = Request.Form["Url"];          
          
            _url = _url.ToLower().Trim();
            if ((!string.IsNullOrEmpty(_url)) && IsValidRssFeed(_url))
            {
                try
                {
                    var rssFeed = new RssFeed { Url = _url };
                    if (_dbContext.RssFeeds.FirstOrDefault(feed => feed.Url == _url) is null)
                    {
                        _dbContext.RssFeeds.Add(rssFeed);
                        await _dbContext.SaveChangesAsync();
                    }
                    else 
                    {
                        ErrorMessage = "URL già presente.";
                    }
                }
                catch (Exception e) { ErrorMessage = e.ToString(); }
            }
            else
            {
                // Gestione dell'errore se l'URL è vuoto
                ErrorMessage = "Inserire un URL valido.";
            }
            OnGet();
        }

        public bool IsValidRssFeed(string url)
        {
            try
            {
                // Effettua una richiesta per ottenere il feed RSS dall'URL
                var reader = XmlReader.Create(url);
                var feed = SyndicationFeed.Load(reader);
                reader.Close();

                // Se non ci sono eccezioni, consideriamo il feed valido
                return true;
            }
            catch
            {
                // Se si verifica un'eccezione durante il caricamento del feed, lo consideriamo non valido
                return false;
            }
        }

        public void OnGet()
        {
            try
            {
                var pageSize = DefaultPageSize;
                string pind = Request.Query["pageIndex"];
                if (pind is null) pind = "1";
                int.TryParse(pind,out PageIndex);

                // Calcola l'offset per il record da cui iniziare
                int skipAmount = (PageIndex - 1) * pageSize;

                int totalItemCount = _dbContext.RssFeeds.Count();
                TotalPages = (int)Math.Ceiling((double)totalItemCount / pageSize);


                // Esegui la query per recuperare i dati paginati
                FeedList = _dbContext.RssFeeds
                                    .OrderBy(x => x.Id) // Ordina per qualche criterio
                                    .Skip(skipAmount)
                                    .Take(pageSize)
                                    .ToList();

                //FeedList = _dbContext.RssFeeds.ToList();
            }
            catch (Exception ex) { ErrorMessage = ex.ToString(); }
        }
    }
}
