using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using sc4ff.Models;

namespace sc4ff
{
    public class DeleteModel : PageModel
    {
        private readonly sc4ff.Models._0bservContext _context;

        public DeleteModel(sc4ff.Models._0bservContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RssFeed RssFeed { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rssfeed = await _context.RssFeeds.FirstOrDefaultAsync(m => m.Id == id);

            if (rssfeed == null)
            {
                return NotFound();
            }
            else
            {
                RssFeed = rssfeed;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rssfeed = await _context.RssFeeds.FindAsync(id);
            if (rssfeed != null)
            {
                RssFeed = rssfeed;
                _context.RssFeeds.Remove(RssFeed);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
