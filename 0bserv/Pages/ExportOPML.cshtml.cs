using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _0bserv.Models;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;

namespace _0bserv.Pages
{
    public class ExportOPMLModel : PageModel
    {
        private readonly _0bservDbContext _context;

        public ExportOPMLModel(_0bservDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                var feeds = _context.RssFeeds.ToList();

                var opmlXml = new XmlDocument();
                var declaration = opmlXml.CreateXmlDeclaration("1.0", "UTF-8", null);
                opmlXml.AppendChild(declaration);

                var opmlElement = opmlXml.CreateElement("opml");
                opmlXml.AppendChild(opmlElement);

                var headElement = opmlXml.CreateElement("head");
                opmlElement.AppendChild(headElement);

                var titleElement = opmlXml.CreateElement("title");
                titleElement.InnerText = "My RSS Feeds";
                headElement.AppendChild(titleElement);

                var bodyElement = opmlXml.CreateElement("body");
                opmlElement.AppendChild(bodyElement);

                foreach (var feed in feeds)
                {
                    var outlineElement = opmlXml.CreateElement("outline");
                    outlineElement.SetAttribute("type", "rss");
                    outlineElement.SetAttribute("text", GetNameFromUrl(feed.Url)); // Utilizziamo il nome del feed ricavato dall'URL
                    outlineElement.SetAttribute("xmlUrl", feed.Url);
                    bodyElement.AppendChild(outlineElement);
                }

                var tempFileName = Path.GetTempFileName();
                opmlXml.Save(tempFileName);

                var fileBytes = System.IO.File.ReadAllBytes(tempFileName);
                var fileName = "feeds.opml";

                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                // Gestire eventuali eccezioni
                return StatusCode(500);
            }
        }

        private string GetNameFromUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                string domainName = uri.Host;
                string[] segments = uri.Segments;
                string feedName = segments.Length > 1 ? segments[segments.Length - 2].TrimEnd('/') : domainName;

                // Aggiungi il nome del dominio al nome del feed
                return $"{domainName} - {feedName}";
            }
            catch (Exception ex)
            {
                return "Nome non disponibile";
            }
        }

    }
}
