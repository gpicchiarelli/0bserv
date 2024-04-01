using System.ServiceModel.Syndication;
using System.Xml;
using _0bserv.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _0bserv.Pages
{
    public class RssFeedModel : PageModel
    {
        public string ErrorMessage { get; set; }
        public List<FeedModel> FeedList { get; set; }
        private readonly _0bservDbContext _dbContext;
        public int PageIndex = 1;
        public int TotalPages = 1;
        public new string? Url { get; set; }
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
                    FeedModel feedModel = new() { Url = _url };
                    FeedModel rssFeed = feedModel;
                    if (_dbContext.RssFeeds.FirstOrDefault(feed => feed.Url == _url) is null)
                    {
                        _ = _dbContext.RssFeeds.Add(rssFeed);
                        _ = await _dbContext.SaveChangesAsync();
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
                XmlReader reader = XmlReader.Create(url);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
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
                int pageSize = DefaultPageSize;
                string pind = Request.Query["pageIndex"];
                pind ??= "1";
                _ = int.TryParse(pind, out PageIndex);

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
