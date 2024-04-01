using _0bserv.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _0bserv.Pages.FeedsMgr
{
    public class DetailsModel : PageModel
    {
        private readonly _0bservDbContext _context;

        public DetailsModel(_0bservDbContext context)
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

            FeedModel? rssfeed = await _context.RssFeeds.FirstOrDefaultAsync(m => m.Id == id);
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
