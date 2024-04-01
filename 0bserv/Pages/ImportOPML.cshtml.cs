using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using _0bserv.Models;

namespace _0bserv.Pages
{
    public class ImportOPMLModel : Controller
    {
        private readonly _0bservDbContext _context;

        public ImportOPMLModel(_0bservDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload()
        {
            var opmlFile = Request.Form.Files.FirstOrDefault();
            if (opmlFile == null || opmlFile.Length == 0)
            {
                ViewBag.Message = "Nessun file selezionato.";
                return View("Index");
            }

            if (opmlFile.ContentType != "text/xml")
            {
                ViewBag.Message = "Il file non è un file OPML valido.";
                return View("Index");
            }

            try
            {
                using (var stream = new StreamReader(opmlFile.OpenReadStream()))
                {
                    XDocument doc = XDocument.Load(stream);
                    var feedUrls = (from outline in doc.Descendants("outline")
                                    select outline.Attribute("xmlUrl")?.Value).ToList();

                    foreach (var url in feedUrls)
                    {
                        _context.RssFeeds.Add(new FeedModel { Url = url });
                    }
                    _context.SaveChanges();

                    ViewBag.Message = $"File OPML importato correttamente. Trovati {feedUrls.Count} feed.";
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Si è verificato un errore durante l'importazione del file OPML: {ex.Message}";
                return View("Index");
            }
        }
    }
}
