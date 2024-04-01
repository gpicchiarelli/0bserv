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
            // Conta solo il numero di elementi senza recuperare tutti i dati
            var totalElementi = _context.FeedContents.Count();

            // Ottieni solo l'ID dei contenuti ordinati per data di pubblicazione in modo discendente
            var idsContenutiOrdinati = _context.FeedContents
                                            .OrderByDescending(contenuto => contenuto.PublishDate)
                                            .Select(contenuto => contenuto.Id)
                                            .ToList();

            // Salta e prendi il numero corretto di ID per la pagina specificata
            var idContenutiPagina = idsContenutiOrdinati
                                        .Skip((pagina - 1) * ElementiPerPagina)
                                        .Take(ElementiPerPagina);

            // Recupera solo i dati necessari per la pagina corrente
            Contenuti = new List<ContenutoViewModel>();
            foreach (var idContenuto in idContenutiPagina)
            {
                var contenuto = _context.FeedContents.Find(idContenuto);
                if (contenuto != null)
                {
                    var nuovoContenuto = new ContenutoViewModel
                    {
                        Id = contenuto.Id,
                        Titolo = Regex.Replace(contenuto.Title, "<.*?>", String.Empty),
                        DataPubblicazione = contenuto.PublishDate
                    };
                    // Limita la lunghezza del titolo per la visibilità
                    nuovoContenuto.Titolo = nuovoContenuto.Titolo.Length > 50 ? nuovoContenuto.Titolo.Substring(0, 50) + "..." : nuovoContenuto.Titolo;

                    Contenuti.Add(nuovoContenuto);
                }
            }
            PaginaCorrente = pagina;
            NumeroPagine = (int)Math.Ceiling((double)totalElementi / ElementiPerPagina);
        }
    }
}