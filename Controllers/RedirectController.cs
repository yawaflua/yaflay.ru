using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using yawaflua.ru.Models;
using yawaflua.ru.Models.Tables;
using yawaflua.ru.Utilities;

namespace yawaflua.ru.Controllers
{
    [Route("/r/")]
    [Authorize]
    public class RedirectController : ControllerBase
    {
        private AppDbContext ctx;
        private MemoryCache cache;
        public RedirectController(AppDbContext ctx, MemoryCache cache) 
        {
            this.ctx = ctx;
            this.cache = cache;
        }
        [HttpGet("{uri}")]
        public async Task<IActionResult> FromGitHub(string uri, [FromQuery(Name = "to")] string? to, [FromQuery(Name = "debug")] string debug = "false")
        {
            Redirects redirects;
            if (!cache.TryGetValue($"redirectsWithUrl-{uri}", out redirects) || ctx.Redirects.TryGetValue(k => k.uri == uri, out redirects) )
                cache.Set($"redirectsWithUrl-{uri}", redirects, DateTime.Now.AddMinutes(20));
            
            return Redirect(redirects?.redirectTo + to?.Replace("%", "/") ?? "/404");

        }
    }
}
