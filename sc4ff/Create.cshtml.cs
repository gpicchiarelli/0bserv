using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using sc4ff.Models;

namespace sc4ff
{
    public class CreateModel : PageModel
    {
        private readonly sc4ff.Models._0bservContext _context;

        public CreateModel(sc4ff.Models._0bservContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public RssFeed RssFeed { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.RssFeeds.Add(RssFeed);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
