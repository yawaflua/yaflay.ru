using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using yawaflua.ru.Models.Tables;
using Microsoft.Extensions.Caching.Memory;
using yawaflua.ru.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using yawaflua.ru.Auth;
using yawaflua.ru.Database.Tables;
using yawaflua.ru.Utilities;
using api.yawaflua.ru.Models.Tables;
using Newtonsoft.Json;

namespace yawaflua.ru.Controllers
{
    [Route("api/")]
    public class ApiController : Controller
    {
        private IMemoryCache cache;
        private AppDbContext ctx;
        public ApiController(IMemoryCache cache, AppDbContext ctx)
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


        [HttpGet("Index")]
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

        [HttpGet("Projects")]
        public async Task<IActionResult> getProjects()
        {
            Console.WriteLine("Im here");
            Projects[] projects = Array.Empty<Projects>();
            if (cache.TryGetValue("projects", out projects) || ctx.Projects.Any())
            {
                projects ??= ctx.Projects.ToArray();
                cache.Set("projects", (object)projects);
            }
            Console.WriteLine(JsonConvert.SerializeObject(projects));
            return Ok(projects);
        }

        [HttpPost("redirects")]
        [Authorize(AuthenticationSchemes = "DISCORD-OAUTH-PRIVATE")]
        public async Task<IActionResult> createRedirectUri([FromQuery]string url, [FromQuery] string uri)
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
            if (response["user"] != null || Startup.ownerId?.FirstOrDefault(response["user"]?["id"].ToString()) == null)
            {
                Redirects redirects = new()
                {
                    redirectTo = url,
                    uri = uri
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
        [HttpPost("Blog")]
        [Authorize(AuthenticationSchemes = "DISCORD-OAUTH-PRIVATE")]
        public async Task<IActionResult> createArticle([FromQuery] string title, [FromQuery] string annotation, [FromQuery] string text, [FromQuery] string image, [FromQuery] string author)
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
            if (response["user"] != null && Startup.ownerId?.FirstOrDefault(response["user"]?["id"].ToString()) != null )
            {
                
                Blogs article = new()
                {
                    Annotation = annotation,
                    authorId = response["user"]["id"].ToString(),
                    dateTime = DateTime.Now,
                    ImageUrl = image,
                    Text = text,
                    Title = title,
                    authorNickname = response["user"]["global_name"].ToString()
                };
                await ctx.Blogs.AddAsync(article);
                await ctx.SaveChangesAsync();
                return Ok();

            }
            else
            {
                return Unauthorized();
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

        [HttpGet("Blog/{blogId?}/comments")]
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
        [HttpPost("Blog/{blogId}/comments")]
        [Authorize(AuthenticationSchemes = "DISCORD-OAUTH-PUBLIC")]

        public async Task<IActionResult> CreateBlogComments(int blogId, [FromQuery] string text, [FromQuery] string sender)
        { 
            Comments comment = new()
            {
                creatorMail = sender,
                dateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Text = text,
                postId = blogId
            };
            await ctx.Comments.AddAsync(comment);
            await ctx.SaveChangesAsync();
            return Ok();
        }
        
        [HttpGet("Blog/{id}")]
        public async Task<IActionResult> blog(int id)
        {
            Blogs? blog;
            if (!cache.TryGetValue($"blogWithId{id}", out blog) && ctx.Blogs.TryGetValue(k => k.Id == id, out blog)) 
                cache.Set($"blogWithId{id}", blog, DateTime.Now.AddMinutes(30));
         
            return Ok(blog);
        }
        [HttpGet("Blog")]
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
        [HttpPost("authorize")]
        public async Task<IActionResult> authorizeUser([FromBody] authorizeBody body)
        {
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
        
        
    }
}
