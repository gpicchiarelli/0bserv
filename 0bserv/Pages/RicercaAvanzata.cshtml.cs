using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _0bserv.Models;
using System.ComponentModel.DataAnnotations;

namespace _0bserv.Pages
{
    public class SearchModel : PageModel
    {
        private readonly _0bservDbContext _context;
        private const int PageSize = 15; // Numero di elementi per pagina
        public List<FeedContentModel> SearchResults { get; set; }

        public SearchModel(_0bservDbContext context)
        {
            _context = context;
            SearchResults = new();
        }

        [BindProperty]
        public SearchInputModel SearchInput { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task<IActionResult> OnGet() 
        {
            // Recupera i parametri di ricerca
            var keyword = SearchInput?.Keyword;
            var startDate = SearchInput?.StartDate;
            var endDate = SearchInput?.EndDate;

            // Verifica se i parametri di ricerca sono validi
            if (string.IsNullOrWhiteSpace(keyword))
            {
                ModelState.AddModelError("SearchInput.Keyword", "Inserire un valore per il filtro");
                return Page();
            }

            // Applica il filtro solo se il valore di keyword non è vuoto
            var query = BuildQuery(keyword, startDate, endDate);

            // Calcola il numero totale di risultati
            var totalResults = await query.CountAsync();

            // Calcola il numero totale di pagine
            TotalPages = (int)Math.Ceiling((double)totalResults / PageSize);

            // Imposta la pagina corrente
            if (Request.Query.ContainsKey("CurrentPage"))
            {
                CurrentPage = Convert.ToInt32(Request.Query["CurrentPage"]);
            }
            else
            {
                CurrentPage = 1;
            }

            // Se la pagina corrente è maggiore del numero totale di pagine, imposta la pagina corrente all'ultima pagina
            if (CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }

            // Se la pagina corrente è minore di 1, imposta la pagina corrente alla prima pagina
            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }

            // Seleziona solo i risultati corrispondenti alla pagina corrente
            SearchResults = await query.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            // Recupera i parametri di ricerca
            var keyword = SearchInput.Keyword;
            var startDate = SearchInput.StartDate;
            var endDate = SearchInput.EndDate;

            // Verifica se i parametri di ricerca sono validi
            if (string.IsNullOrWhiteSpace(keyword))
            {
                ModelState.AddModelError("SearchInput.Keyword", "Inserire un valore per il filtro");
                return Page();
            }

            // Applica il filtro solo se il valore di keyword non è vuoto
            var query = BuildQuery(keyword, startDate, endDate);

            // Calcola il numero totale di risultati
            var totalResults = await query.CountAsync();

            // Calcola il numero totale di pagine
            TotalPages = (int)Math.Ceiling((double)totalResults / PageSize);

            // Imposta la pagina corrente
            if (Request.Query.ContainsKey("CurrentPage"))
            {
                CurrentPage = Convert.ToInt32(Request.Query["CurrentPage"]);
            }
            else
            {
                CurrentPage = 1;
            }

            // Se la pagina corrente è maggiore del numero totale di pagine, imposta la pagina corrente all'ultima pagina
            if (CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }

            // Se la pagina corrente è minore di 1, imposta la pagina corrente alla prima pagina
            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }

            // Seleziona solo i risultati corrispondenti alla pagina corrente
            SearchResults = await query.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();

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
        [Required(ErrorMessage = "Il campo parola chiave è obbligatorio.")]
        public string Keyword { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
