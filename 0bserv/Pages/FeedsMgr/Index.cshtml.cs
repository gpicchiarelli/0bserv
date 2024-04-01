using _0bserv.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _0bserv.Pages.FeedsMgr
{
    public class FeedIndexModel : PageModel
    {
        private readonly _0bservDbContext _context;

        public FeedIndexModel(_0bservDbContext context)
        {
            _context = context;
        }

        public IList<FeedModel> RssFeed { get; set; } = default!;

        public async Task OnGetAsync()
        {
            RssFeed = await _context.RssFeeds.ToListAsync();
        }
    }
}
