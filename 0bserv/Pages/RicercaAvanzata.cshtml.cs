using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _0bserv.Models;
using Microsoft.EntityFrameworkCore;

namespace _0bserv.Pages
{
    public class SearchModel : PageModel
    {
        private readonly _0bservDbContext _context;

        public SearchModel(_0bservDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SearchInputModel SearchInput { get; set; }

        public List<FeedContentModel> SearchResults { get; set; }
        public int PaginaCorrente { get; set; }
        public int NumeroPagine { get; set; }

        public void OnGet(int? pagina, string keyword, DateTime? startDate, DateTime? endDate)
        {
            // Inizializza l'oggetto SearchInputModel
            SearchInput = new SearchInputModel
            {
                Keyword = keyword,
                StartDate = startDate,
                EndDate = endDate
            };

            // Esegui la ricerca e la paginazione
            PaginaCorrente = pagina ?? 1;
            int risultatiPerPagina = 10;
            var query = BuildQuery();

            SearchResults = query.Skip((PaginaCorrente - 1) * risultatiPerPagina)
                                 .Take(risultatiPerPagina)
                                 .ToList();

            NumeroPagine = (int)Math.Ceiling((double)query.Count() / risultatiPerPagina);
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


        private IQueryable<FeedContentModel> BuildQuery()
        {
            var query = _context.FeedContents.AsQueryable();

            if (!string.IsNullOrEmpty(SearchInput.Keyword))
            {
                query = query.Where(f =>
                    EF.Functions.Like(f.Title, $"%{SearchInput.Keyword}%") ||
                    EF.Functions.Like(f.Description, $"%{SearchInput.Keyword}%") ||
                    EF.Functions.Like(f.Link, $"%{SearchInput.Keyword}%") ||
                    EF.Functions.Like(f.Author, $"%{SearchInput.Keyword}%"));
            }

            if (SearchInput.StartDate.HasValue)
            {
                query = query.Where(f => f.PublishDate >= SearchInput.StartDate);
            }

            if (SearchInput.EndDate.HasValue)
            {
                query = query.Where(f => f.PublishDate <= SearchInput.EndDate);
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
