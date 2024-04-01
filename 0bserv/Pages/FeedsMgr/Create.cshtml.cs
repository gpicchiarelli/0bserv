using System.ServiceModel.Syndication;
using System.Xml;
using _0bserv.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _0bserv.Pages.FeedsMgr
{
    public class CreateModel : PageModel
    {
        private readonly _0bservDbContext _context;
        public string ErrorMessage = "";
        public new string Url;

        public CreateModel(_0bservDbContext context)
        {
            _context = context;
            _ = _context.RssFeeds.ToListAsync();

        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public string RssFeed { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            RssFeed = RssFeed.ToLower().Trim();
            if (IsValidRssFeed(RssFeed))
            {
                if (_context.RssFeeds.FirstOrDefault(feed => feed.Url == RssFeed) is null)
                {
                    _ = _context.RssFeeds.Add(new FeedModel { Url = RssFeed });
                    _ = await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
                else
                {
                    ErrorMessage = "Feed già presente";
                }
            }
            else
            {
                ErrorMessage = "URL non valido";
            }
            return Page();
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
    }
}
