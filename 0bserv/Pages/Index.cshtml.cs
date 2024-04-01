using _0bserv.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using BootstrapBlazor.Components;

namespace _0bserv.Pages
{
    public class ContenutoViewModel
    {
        public int Id { get; set; }
        public string Titolo { get; set; }
        public DateTime? DataPubblicazione {get; set;}
    }

    public class IndexModel : PageModel
    {
        private const int ElementiPerPagina = 30; // Modifica il numero di elementi per pagina se necessario
        private readonly _0bservDbContext _context;

        public IndexModel(_0bservDbContext context)
        {
            _context = context;
        }

        public List<ContenutoViewModel> Contenuti { get; set; }
        public int PaginaCorrente { get; set; }
        public int NumeroPagine { get; set; }

        public void OnGet(int pagina = 1)
        {
            var totalElementi = _context.FeedContents.Count(); // Esempio: il totale degli elementi nella tabella

            Contenuti = new List<ContenutoViewModel>();

            foreach (var contenuto in _context.FeedContents.Skip((pagina - 1) * ElementiPerPagina).Take(ElementiPerPagina))
            {
                var nuovoContenuto = new ContenutoViewModel
                {
                    Id = contenuto.Id,
                    // Rimuovi la formattazione HTML dal titolo
                    Titolo = Regex.Replace(contenuto.Title, "<.*?>", String.Empty),
                    // Assegna la data di pubblicazione
                    DataPubblicazione = contenuto.PublishDate
                };
                // Limita la lunghezza del titolo per la visibilità
                nuovoContenuto.Titolo = nuovoContenuto.Titolo.Length > 50 ? nuovoContenuto.Titolo.Substring(0, 50) + "..." : nuovoContenuto.Titolo;

                Contenuti.Add(nuovoContenuto);
            }

            PaginaCorrente = pagina;
            NumeroPagine = (int)Math.Ceiling((double)totalElementi / ElementiPerPagina);
        }

    }
}