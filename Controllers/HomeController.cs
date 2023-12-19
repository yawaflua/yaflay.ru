using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        /// <summary>
        /// Getting redirect url from github file
        /// </summary>
        /// <param name="baseUrl"> uri-key of json in github file</param>
        /// <returns type="string">url</returns>
        private async Task<string?> getUrlFromGit(string baseUrl)
        {
            try
            {
                HttpClient client = new();
                HttpResponseMessage getter = await client.GetAsync("https://raw.githubusercontent.com/yawaflua/yaflay.ru/master/redirect_uris.json");
                JsonDocumentOptions jsonDocumentOptions = new ()
                {
                    AllowTrailingCommas = true
                };
                JsonNode? allFile = JsonNode.Parse(await getter.Content.ReadAsStringAsync(),
                                                 documentOptions: jsonDocumentOptions);
                ;
                return (string?)allFile[baseUrl];
            }
            catch (Exception except)
            {
                await Console.Out.WriteLineAsync(except.Message.ToString());
                return null;
            }
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
            if (response["user"] != null || response["user"]?["id"].ToString() == "945317832290336798")
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
            if (response["user"] != null || response["user"]?["id"].ToString() == "945317832290336798")
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

            Blogs? blog = Startup.dbContext.Blogs.FirstOrDefault(k => k.Id == blogId) ?? null;
            return Ok(blog);
        }

        [HttpGet("{uri}")]
        public async Task<IActionResult> FromGitHub(string uri)
        {
            
            string? url = Startup.dbContext.Redirects.FirstOrDefault(k => k.uri == uri)?.redirectTo;
            if (url != null) 
            {
                return Redirect(url);
            }
            else
            {
                return Redirect("/404");
            }
            
        }
        
    }
}
