using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _0bserv.DbContexts;
using _0bserv.Models;
using Microsoft.EntityFrameworkCore;

namespace _0bserv.Pages
{
    public class RssFeedModel : PageModel
    {
        public string ErrorMessage { get; set; }
        public List<RssFeed> FeedList { get; set; }
        private readonly _0bservDbContext _dbContext;
        public int PageIndex = 0;
        public int TotalPages = 0;

        public RssFeedModel(_0bservDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void OnGet()
        {
            try
            {
                FeedList = _dbContext.RssFeeds.ToList();
            }
            catch (Exception ex) { ErrorMessage = ex.ToString(); }
        }
    }
}
