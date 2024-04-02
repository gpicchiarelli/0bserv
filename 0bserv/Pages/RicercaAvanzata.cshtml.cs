using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _0bserv.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace _0bserv.Pages
{
    public class SearchModel : PageModel
    {
        private readonly _0bservDbContext _context;
        private readonly IMemoryCache _cache;

        public SearchModel(_0bservDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [BindProperty]
        public SearchInputModel SearchInput { get; set; }

        public List<FeedContentModel> SearchResults { get; set; }
        public int PaginaCorrente { get; set; }
        public int NumeroPagine { get; set; }

        public async Task<IActionResult> OnGetAsync(int? pagina, string keyword, DateTime? startDate, DateTime? endDate)
        {
            SearchInput = new SearchInputModel
            {
                Keyword = keyword,
                StartDate = startDate,
                EndDate = endDate
            };

            // Esegui la ricerca
            var searchResults = await BuildQuery(keyword, startDate, endDate).ToListAsync();

            // Applica la paginazione ai risultati filtrati
            PaginaCorrente = pagina ?? 1;
            int risultatiPerPagina = 10;
            SearchResults = searchResults
                .Skip((PaginaCorrente - 1) * risultatiPerPagina)
                .Take(risultatiPerPagina)
                .ToList();

            NumeroPagine = (int)Math.Ceiling((double)searchResults.Count / risultatiPerPagina); // Assumendo 10 risultati per pagina

            return Page();
        }



        public IActionResult OnPostSearch()
        {
            // Verifica se la keyword è vuota
            if (string.IsNullOrWhiteSpace(SearchInput.Keyword))
            {
                // Se la keyword è vuota, reindirizza alla pagina iniziale
                return RedirectToPage("/Index");
            }

            // Verifica e valorizza le date se sono nulle
            if (!SearchInput.StartDate.HasValue)
            {
                SearchInput.StartDate = DateTime.Today;
            }

            if (!SearchInput.EndDate.HasValue)
            {
                SearchInput.EndDate = DateTime.Today;
            }

            // Costruisci l'URL con i parametri di ricerca
            var url = Url.Page("/Search", new
            {
                pagina = 1,
                keyword = SearchInput.Keyword,
                startDate = SearchInput.StartDate,
                endDate = SearchInput.EndDate
            });

            // Reindirizza alla pagina di ricerca con i parametri di ricerca nell'URL
            return Redirect(url);
        }

        private IQueryable<FeedContentModel> BuildQuery(string keyword, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.FeedContents.AsQueryable();

            // Applica i filtri solo se i valori non sono nulli
            if (!string.IsNullOrEmpty(keyword))
            {
                string keywordPattern = $"%{keyword}%";
                query = query.Where(f =>
                    EF.Functions.Like(f.Title, keywordPattern) ||
                    EF.Functions.Like(f.Description, keywordPattern) ||
                    EF.Functions.Like(f.Link, keywordPattern) ||
                    EF.Functions.Like(f.Author, keywordPattern));
            }

            if (startDate.HasValue)
            {
                query = query.Where(f => f.PublishDate >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(f => f.PublishDate <= endDate);
            }

            // Ordina i risultati per data di pubblicazione decrescente
            query = query.OrderByDescending(f => f.PublishDate);

            return query;
        }


        private string GetCacheKey(int? pagina, string keyword, DateTime? startDate, DateTime? endDate)
        {
            return $"Search_{pagina}_{keyword}_{startDate?.ToString("yyyyMMdd")}_{endDate?.ToString("yyyyMMdd")}";
        }
    }

    public class SearchInputModel
    {
        public string Keyword { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Category { get; set; }
    }
}
