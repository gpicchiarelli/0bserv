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
    public class IndexModel : PageModel
    {
        private readonly sc4ff.Models._0bservContext _context;

        public IndexModel(sc4ff.Models._0bservContext context)
        {
            _context = context;
        }

        public IList<RssFeed> RssFeed { get;set; } = default!;

        public async Task OnGetAsync()
        {
            RssFeed = await _context.RssFeeds.ToListAsync();
        }
    }
}
