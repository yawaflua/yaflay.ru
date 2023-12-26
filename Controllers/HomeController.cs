using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;
using System.Text.Json.Nodes;
using yaflay.ru.Models.Tables;
using Microsoft.Extensions.Caching.Memory;
using yaflay.ru.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using yaflay.ru.Auth;
using yaflay.ru.Database.Tables;

namespace yaflay.ru.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private IMemoryCache cache;
        private AppDbContext ctx;
        public HomeController(IMemoryCache cache, AppDbContext ctx)
        {
            this.cache = cache;
            this.ctx = ctx;
        }
        public class authorizeBody
        {
            public string melon { get; set; }
            public string watermelon { get; set; }
            public string discordId { get; set; }
            public ApiKeyTypes type { get; set; }
            
        }
        public class commentBody
        {
            public string text { get; set; }
            public string sender { get; set; }
        }
        public class articleBody
        {
            public string title { get; set; }
            public string annotation { get; set; }
            public string text { get; set; }
            public string image { get; set; }
            public string author { get; set; } 
        }
        public class redirectBody
        {
            public string url { get; set; }
            public string uri { get; set; }
            public string author { get; set; }
        }

        [HttpGet("api/Index")]
        public async Task<IActionResult> getIndexPage()
        {
            string? indexPage = cache.Get<string>($"indexPage");
            if (indexPage == null)
            {
                indexPage = await Startup.client.GetStringAsync(Startup.readmeFile);
                if (indexPage != null)
                    cache.Set($"indexPage", (object)indexPage, DateTime.Now.AddMinutes(10));
            }

            return Ok(indexPage);
        }

        [HttpPost("api/redirects")]
        [Authorize(AuthenticationSchemes = "DISCORD-OAUTH-PRIVATE")]
        public async Task<IActionResult> createRedirectUri([FromBody]redirectBody body)
        {
            Console.WriteLine("url" + body.uri);
            HttpResponseMessage message;
            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "https://discordapp.com/api/oauth2/@me"))
            {
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Cookies["melon"]); ;
                message = await Startup.client.SendAsync(requestMessage);
            }
            string responseBody = await message.Content.ReadAsStringAsync();
            JsonNode response = JsonNode.Parse(responseBody);
            if (response["user"] != null || Startup.ownerId.FirstOrDefault(response["user"]?["id"].ToString()) == null)
            {
                Redirects redirects = new()
                {
                    redirectTo = body.url,
                    uri = body.uri
                };
                await ctx.Redirects.AddAsync(redirects);
                await ctx.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpPost("api/Blog")]
        [Authorize(AuthenticationSchemes = "DISCORD-OAUTH-PRIVATE")]
        public async Task<IActionResult> createArticle([FromBody] articleBody body)
        {

            HttpResponseMessage message;
            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "https://discordapp.com/api/oauth2/@me"))
            {
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Cookies["melon"]); ;
                message = await Startup.client.SendAsync(requestMessage);
            }
            string responseBody = await message.Content.ReadAsStringAsync();
            JsonNode response = JsonNode.Parse(responseBody);
            if (response["user"] != null || Startup.ownerId.FirstOrDefault(response["user"]?["id"].ToString()) == null )
            {
                try
                {
                    Blogs article = new()
                    {
                        Annotation = body.annotation,
                        authorId = response["user"]["id"].ToString(),
                        dateTime = DateTime.Now,
                        ImageUrl = body.image,
                        Text = body.text,
                        Title = body.title,
                        authorNickname = response["user"]["global_name"].ToString()
                    };
                    await ctx.Blogs.AddAsync(article);
                    await ctx.SaveChangesAsync();
                    return Ok(body);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error: HomeController error");
                    Console.WriteLine("debug: lines: 80-96");
                    Console.WriteLine($"debug: data from site: {body}");
                    Console.WriteLine($"debug: exception: {ex.Message}");
                    return StatusCode(500, body);
                }
            }
            else
            {
                return Unauthorized(body);
            }
        }
        [HttpGet("logout")]
        public async Task<IActionResult> authorizeDiscord()
        {
            Response.Cookies.Delete("melon");
            Response.Cookies.Delete("watermelon");
            Response.Cookies.Delete("cable");
            return Redirect("/");
        }

        [HttpGet("api/Blog/{blogId?}/comments")]
        public async Task<IActionResult> blogComments(int? blogId)
        {
            Comments[]? comments = (Comments[]?)cache.Get($"commentsWithBlogId{blogId}");
            if (comments == null)
            {
                comments = ctx.Comments.Where(k => k.postId == blogId).ToArray();
                if (comments != null)
                    cache.Set($"commentsWithBlogId{blogId}", (object[])comments, DateTime.Now.AddMinutes(5));
            }
            
            return Ok(comments);
        }
        [HttpPost("api/Blog/{blogId}/comments")]
        [Authorize(AuthenticationSchemes = "DISCORD-OAUTH-PUBLIC")]

        public async Task<IActionResult> CreateBlogComments(int blogId, [FromBody]commentBody body)
        { 
            Comments comment = new()
            {
                creatorMail = body.sender,
                dateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Text = body.text,
                postId = blogId
            };
            await ctx.Comments.AddAsync(comment);
            await ctx.SaveChangesAsync();
            return Ok();
        }
        
        [HttpGet("api/Blog/{blogId}")]
        public async Task<IActionResult> blog(int blogId)
        {
            Blogs? blog = cache.Get<Blogs>($"blogWithId{blogId}");
            if (blog == null)
            {
                blog = ctx.Blogs.FirstOrDefault(k => k.Id == blogId);
                if (blog != null)
                    cache.Set($"blogWithId{blogId}", (object)blog, DateTime.Now.AddMinutes(10));
                
                
            }
            
            
            return Ok(blog);
        }
        [HttpGet("api/Blog")]
        public async Task<IActionResult> allBlogs()
        {
            Blogs[]? blogs = cache.Get<Blogs[]>($"allBlogs");
            if (blogs == null)
            {
                blogs = ctx.Blogs.ToArray();
                if (blogs != null)
                    cache.Set($"allBlogs", (object)blogs, DateTime.Now.AddMinutes(10));


            }
            return Ok(blogs);
        }
        [HttpPost("api/authorize")]
        public async Task<IActionResult> authorizeUser([FromBody] authorizeBody body)
        {
            var fromCache = cache.Get<ApiKey>($"apiKey-melon-{body.melon}");
            if (fromCache == null)
            {
                var melon = ctx.ApiKeys.FirstOrDefault(k => k.Melon == body.melon);
                if (melon != null)
                {
                    cache.Set($"apiKey-melon-{body.melon}", (object)melon, DateTime.Now.AddMinutes(20));
                }
            }

            await ctx.ApiKeys.AddAsync(
                new()
                {
                    DiscordOwnerId = ulong.Parse(body.discordId),
                    Key = body.melon,
                    Melon = body.melon,
                    Type = ApiKeyTypes.Public
                }
                );
            await ctx.SaveChangesAsync();
            return Ok(body.melon);
        }
        [HttpGet("{uri}")]
        public async Task<IActionResult> FromGitHub(string uri)
        {
            Console.WriteLine(uri);
            //if (uri == "404") { return Ok(); }
            Redirects? fromCache = cache.Get<Redirects>($"redirectsWithUrl-{uri}") ?? null;
            if (fromCache == null)
            {
                fromCache = ctx.Redirects.FirstOrDefault(k => k.uri == uri);
                Console.WriteLine("Im here!");
                if (fromCache != null)
                    cache.Set($"redirectsWithUrl-{uri}", (object)fromCache, DateTime.Now.AddMinutes(10));
            }
            Console.WriteLine(fromCache.ToString());
            return Redirect(fromCache?.redirectTo ?? "/404");
           
        } 
        
    }
}
