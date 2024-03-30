using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _0bserv.Pages
{
    public class LicenzaModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public LicenzaModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
