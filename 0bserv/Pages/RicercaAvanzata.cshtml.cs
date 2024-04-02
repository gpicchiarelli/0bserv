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

        public void OnGet(int? pagina)
        {
            // Inizializza l'oggetto SearchInputModel
            SearchInput = new SearchInputModel();

            // Esegui la ricerca e la paginazione
            PaginaCorrente = pagina ?? 1;
            int risultatiPerPagina = 10;
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

            SearchResults = query.Skip((PaginaCorrente - 1) * risultatiPerPagina)
                                 .Take(risultatiPerPagina)
                                 .ToList();

            NumeroPagine = (int)Math.Ceiling((double)query.Count() / risultatiPerPagina);
        }

        public void OnPostSearch()
        {
            // Esegui la ricerca
            OnGet(1);
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
