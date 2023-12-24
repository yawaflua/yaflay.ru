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

namespace yaflay.ru.Новая_папка
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
            string? indexPage = (string)cache.Get($"indexPage");
            if (indexPage == null)
            {
                indexPage = await Startup.client.GetStringAsync(Startup.readmeFile);
                if (indexPage != null)
                    cache.Set($"indexPage", (object)indexPage, DateTime.Now.AddMinutes(10));
            }

            return Ok(indexPage);
        }

        [HttpPost("api/redirects")]
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
            Blogs? blog = (Blogs)cache.Get($"blogWithId{blogId}");
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
            Blogs[]? blogs = (Blogs[])cache.Get($"allBlogs");
            if (blogs == null)
            {
                blogs = ctx.Blogs.ToArray();
                if (blog != null)
                    cache.Set($"allBlogs", (object)blogs, DateTime.Now.AddMinutes(10));


            }
            return Ok(blogs);
        }
        [HttpGet("{uri}")]
        public async Task<IActionResult> FromGitHub(string uri)
        {
            Redirects? fromCache = (Redirects)cache.Get($"redirectsWithUrl={uri}");
            if (fromCache != null)
            {
                fromCache = ctx.Redirects.FirstOrDefault(k => k.uri == uri);
                if (fromCache == null)
                    cache.Set($"redirectsWithUrl={uri}", (object)fromCache, DateTime.Now.AddMinutes(5));
            }
           
            return Redirect(fromCache?.redirectTo ?? "/404");
           
        } 
        
    }
}
