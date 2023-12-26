using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace yaflay.ru.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        //public string? uri { get; set; } = null;
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            
        }

        public void OnGet(/**string? uri**/)
        {
            Page();
            //this.uri = uri ?? null;
        }

    }
}