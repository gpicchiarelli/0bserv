using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging; // Aggiungi questo namespace
using _0bserv.Models;
using System;
using System.Text.RegularExpressions;

namespace _0bserv.Pages
{
    public class DettaglioItemModel : PageModel
    {
        private readonly _0bservDbContext _context;
        private readonly ILogger<DettaglioItemModel> _logger; // Inietta ILogger
        public Contenuto Contenuto { get; set; }


        public DettaglioItemModel(_0bservDbContext context, ILogger<DettaglioItemModel> logger)
        {
            _context = context;
            _logger = logger; // Inizializza logger
            Contenuto = new();
            Contenuto.Titolo = String.Empty;
            Contenuto.DataPubblicazione = null;
            Contenuto.Autore = String.Empty;
            Contenuto.Testo = String.Empty;
        }
        public void OnGet()
        {
            try
            {
                string id = Request.Query["idd"];
                id = id.Trim().ToLower();
                if (id != String.Empty)
                {
                    var f = _context.FeedContents.Find(int.Parse(id));
                    Contenuto.Titolo = Regex.Replace(f.Title, "<.*?>", String.Empty);
                    Contenuto.DataPubblicazione = f.PublishDate;
                    Contenuto.Autore = Regex.Replace(f.Author, "<.*?>", String.Empty); ;
                    Contenuto.Testo = Regex.Replace(f.Description, "<.*?>", String.Empty);
                    Contenuto.Collegamento = f.Link;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Si è verificato un errore durante il recupero del contenuto.");
                throw; // Rilancia l'eccezione per mantenere il comportamento predefinito di ASP.NET Core
            }
        }
    }
}