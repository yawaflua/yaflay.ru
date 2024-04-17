using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using yawaflua.ru.Models;
using yawaflua.ru.Models.Tables;

namespace yawaflua.ru.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public IMemoryCache cache;
        public AppDbContext ctx;
        public IndexModel(ILogger<IndexModel> logger, IMemoryCache cache, AppDbContext ctx)
        {
            _logger = logger;
            this.cache = cache;
            this.ctx = ctx;
        }

        public void OnGet()
        {

            Page();
            
        }

    }
}