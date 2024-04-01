using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using _0bserv.Models;

namespace _0bserv.Pages
{
    public class FeedIndexModel : PageModel
    {
        private readonly _0bserv.Models._0bservDbContext _context;

        public FeedIndexModel(_0bserv.Models._0bservDbContext context)
        {
            _context = context;
        }

        public IList<FeedModel> RssFeed { get;set; } = default!;

        public async Task OnGetAsync()
        {
            RssFeed = await _context.RssFeeds.ToListAsync();
        }
    }
}
