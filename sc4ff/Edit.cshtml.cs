using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sc4ff.Models;

namespace sc4ff
{
    public class EditModel : PageModel
    {
        private readonly sc4ff.Models._0bservContext _context;

        public EditModel(sc4ff.Models._0bservContext context)
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

            var rssfeed =  await _context.RssFeeds.FirstOrDefaultAsync(m => m.Id == id);
            if (rssfeed == null)
            {
                return NotFound();
            }
            RssFeed = rssfeed;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(RssFeed).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RssFeedExists(RssFeed.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool RssFeedExists(int id)
        {
            return _context.RssFeeds.Any(e => e.Id == id);
        }
    }
}
