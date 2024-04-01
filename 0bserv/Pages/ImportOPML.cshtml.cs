using System.Collections.Generic;
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
                ViewBag.Message = "File non valido o vuoto.";
                return View("Index");
            }

            List<string> feedUrls = new List<string>();

            using (var stream = new StreamReader(opmlFile.OpenReadStream()))
            {
                XDocument doc = XDocument.Load(stream);
                feedUrls = (from outline in doc.Descendants("outline")
                            select outline.Attribute("xmlUrl")?.Value).ToList();
            }

            // Salva gli URL dei feed nel database o esegui altre operazioni
            foreach (var url in feedUrls)
            {
                _context.RssFeeds.Add(new FeedModel { Url = url });
            }
            _context.SaveChanges();

            ViewBag.Message = $"File OPML importato correttamente. Trovati {feedUrls.Count} feed.";
            return View("Index");
        }
    }
}
