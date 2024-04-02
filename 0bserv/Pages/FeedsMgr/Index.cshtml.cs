using _0bserv.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _0bserv.Pages.FeedsMgr
{
    public class FeedIndexModel : PageModel
    {
        private readonly _0bservDbContext _context;

        public FeedIndexModel(_0bservDbContext context)
        {
            _context = context;
        }

        public IList<FeedModel> RssFeed { get; set; } = default!;
        public int PaginaCorrente { get; set; }
        public int PaginaTotale { get; set; }
        public int InizioPagina { get; set; }
        public int FinePagina { get; set; }

        public async Task OnGetAsync(int pagina = 1)
        {
            const int risultatiPerPagina = 10;

            var totalItems = await _context.RssFeeds.CountAsync();
            PaginaTotale = (int)Math.Ceiling((double)totalItems / risultatiPerPagina);

            PaginaCorrente = pagina;
            InizioPagina = Math.Max(1, PaginaCorrente - 2);
            FinePagina = Math.Min(PaginaTotale, PaginaCorrente + 2);

            RssFeed = await _context.RssFeeds
                .Skip((PaginaCorrente - 1) * risultatiPerPagina)
                .Take(risultatiPerPagina)
                .ToListAsync();
        }
    }
}