using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using yaflay.ru.Models;
using yaflay.ru.Models.Tables;

namespace yaflay.ru.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public IMemoryCache cache;
        public AppDbContext ctx;
        public string? uri { get; set; } = null;
        public IndexModel(ILogger<IndexModel> logger, IMemoryCache cache, AppDbContext ctx)
        {
            _logger = logger;
            this.cache = cache;
            this.ctx = ctx;
        }

        public void OnGet(string? uri)
        {
            
            this.uri = uri ?? null;
            Console.WriteLine(uri);
            if (this.uri != null)
            {
                Redirects? fromCache = cache.Get<Redirects>($"redirectsWithUrl-{uri}") ?? null;
                if (fromCache == null)
                {
                    fromCache = ctx.Redirects.FirstOrDefault(k => k.uri == uri);
                    Console.WriteLine("Im here!");
                    if (fromCache != null)
                        cache.Set($"redirectsWithUrl-{uri}", (object)fromCache, DateTime.Now.AddMinutes(10));
                }
                Console.WriteLine(fromCache?.ToString());
                Response.Redirect(fromCache?.redirectTo ?? "/404");
            }
            else
            {
                Page();
            }
        }

    }
}