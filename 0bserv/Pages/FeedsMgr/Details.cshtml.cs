using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using _0bserv.Models;

namespace _0bserv
{
    public class DetailsModel : PageModel
    {
        private readonly _0bserv.Models._0bservDbContext _context;

        public DetailsModel(_0bserv.Models._0bservDbContext context)
        {
            _context = context;
        }

        public FeedModel RssFeed { get; set; } = default!;

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
    }
}
