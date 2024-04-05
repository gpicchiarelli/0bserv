using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _0bserv.Models;

namespace _0bserv.Pages
{
    public class SearchModel : PageModel
    {
        private readonly _0bservDbContext _context;
        private const int PageSize = 10; // Numero di elementi per pagina

        public SearchModel(_0bservDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SearchInputModel SearchInput { get; set; }
        public List<FeedContentModel> SearchResults { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task<IActionResult> OnPost()
        {
            // Recupera i parametri di ricerca
            var keyword = SearchInput.Keyword;
            var startDate = SearchInput.StartDate;
            var endDate = SearchInput.EndDate;

            var searchResults = await BuildQuery(keyword, startDate, endDate).ToListAsync();

            // Calcola il numero totale di pagine
            TotalPages = (int)Math.Ceiling((double)searchResults.Count / PageSize);

            // Imposta la pagina corrente a 1
            CurrentPage = 1;

            // Imposta i risultati della ricerca per la pagina corrente
            SearchResults = searchResults.Take(PageSize).ToList();

            return Page();
        }

        private IQueryable<FeedContentModel> BuildQuery(string keyword, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.FeedContents.AsQueryable();

            // Applica i filtri solo se i valori non sono nulli o vuoti
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
    }

    public class SearchInputModel
    {
        public string Keyword { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Category { get; set; }
    }
}
