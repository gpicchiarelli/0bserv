using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using _0bserv.Models;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using Microsoft.AspNetCore.Identity.Data;

namespace _0bserv
{
    public class CreateModel : PageModel
    {
        private readonly _0bserv.Models._0bservDbContext _context;
        public string ErrorMessage = "";
        public String Url;

        public CreateModel(_0bserv.Models._0bservDbContext context)
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
            if (IsValidRssFeed(RssFeed))
            {
                if (_context.RssFeeds.FirstOrDefault(feed => feed.Url == RssFeed) is null)
                {
                    _context.RssFeeds.Add(new FeedModel { Url = RssFeed });
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ErrorMessage = "Feed già presente";
                }
                if (!ModelState.IsValid)
                {
                    return Page();
                }
            }
            else 
            {
                
            }
            return Page();
            //return RedirectToPage("./Index");
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
    }
}
