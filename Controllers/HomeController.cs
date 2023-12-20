using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;
using System.Text.Json.Nodes;
using yaflay.ru.Models.Tables;
using yaflay.ru.Pages;

namespace yaflay.ru.Новая_папка
{
    [Route("")]
    public class HomeController : Controller
    {        
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
            if (response["user"] != null || response["user"]?["id"].ToString() == Startup.ownerId)
            {
                Redirects redirects = new()
                {
                    redirectTo = body.url,
                    uri = body.uri
                };
                await Startup.dbContext.Redirects.AddAsync(redirects);
                await Startup.dbContext.SaveChangesAsync();
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
            if (response["user"] != null || response["user"]?["id"].ToString() == Startup.ownerId)
            {
                Author author = new()
                {
                    discordId = ulong.Parse(response["user"]["id"].ToString()),
                    discordNickName = response["user"]["global_name"].ToString()
                };
                Blogs article = new()
                {
                    Annotation = body.annotation,
                    author = author,
                    dateTime = DateTime.Now,
                    ImageUrl = body.image,
                    Text = body.text,
                    Title = body.title
                };
                await Startup.dbContext.Blogs.AddAsync(article);
                await Startup.dbContext.SaveChangesAsync();
                return Ok(body);
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
            
            Comments[] comments = Startup.dbContext.Comments.Where(k => k.postId == blogId).ToArray();
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
            await Startup.dbContext.Comments.AddAsync(comment);
            await Startup.dbContext.SaveChangesAsync();
            return Ok();
        }
        
        [HttpGet("api/Blog/{blogId}")]
        public async Task<IActionResult> blog(int blogId)
        {

            Blogs? blog = Startup.dbContext.Blogs.FirstOrDefault(k => k.Id == blogId);
            return Ok(blog);
        }

        [HttpGet("{uri}")]
        public async Task<IActionResult> FromGitHub(string uri)
        {
            
            string? url = Startup.dbContext.Redirects.FirstOrDefault(k => k.uri == uri)?.redirectTo;
            return Redirect(url ?? "/404");
            
            
            
        }
        
    }
}
