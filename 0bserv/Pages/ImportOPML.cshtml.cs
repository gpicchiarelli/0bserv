using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _0bserv.Models;
using System.Xml.Linq;

namespace _0bserv.Pages
{
    public class ImportOPMLModel : PageModel
    {
        private readonly _0bservDbContext _context;
        private readonly ILogger<ImportOPMLModel> _logger;

        public ImportOPMLModel(_0bservDbContext context, ILogger<ImportOPMLModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public IFormFile OpmlFile { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (OpmlFile == null || OpmlFile.Length == 0)
            {
                TempData["FileUploadError"] = "Nessun file selezionato.";
                return Page();
            }

            if (!OpmlFile.FileName.EndsWith(".opml", StringComparison.OrdinalIgnoreCase))
            {
                TempData["FileUploadError"] = "Il file selezionato non è un file OPML valido.";
                return Page();
            }

            try
            {
                var feedUrls = new List<string>();

                using (var stream = new StreamReader(OpmlFile.OpenReadStream()))
                {
                    var doc = XDocument.Load(stream);
                    feedUrls = (from outline in doc.Descendants("outline")
                                select outline.Attribute("xmlUrl")?.Value).ToList();
                }

                foreach (var url in feedUrls)
                {
                    _context.RssFeeds.Add(new FeedModel { Url = url });
                }

                await _context.SaveChangesAsync();

                TempData["Message"] = $"File OPML importato correttamente. Trovati {feedUrls.Count} feed.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del file OPML.");
                TempData["FileUploadError"] = "Si è verificato un errore durante il caricamento del file OPML.";
            }

            return Page();
        }
    }
}
