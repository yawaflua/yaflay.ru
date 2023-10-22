using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace yaflay.ru.Новая_папка
{
    [Route("")]
    public class HomeController : Controller
    {
        // GET: HomeController

        private async Task<string> getUrlFromGit(string baseUrl)
        {
            try
            {
                HttpClient client = new();
                HttpResponseMessage getter = await client.GetAsync("https://raw.githubusercontent.com/YaFlay/yaflay.ru/master/redirect_uris.json");
                await Console.Out.WriteLineAsync(await getter.Content.ReadAsStringAsync());
                JsonNodeOptions jsonNodeOptions = new ();
                JsonDocumentOptions jsonDocumentOptions = new();
                jsonDocumentOptions.AllowTrailingCommas = true;
                JsonNode allFile = JsonNode.Parse(await getter.Content.ReadAsStringAsync(), jsonNodeOptions, jsonDocumentOptions);
                return (string?)allFile[baseUrl];
            }
            catch (Exception except)
            {
                await Console.Out.WriteLineAsync(except.ToString());
                return null;
            }
        }
        
        // GET: HomeController/Details/5
        [HttpGet("{uri}")]
        public async Task<IActionResult> fromGitHub(string? uri)
        {
            
            if (uri == "Robots.txt") { return Ok("User-Agent: * \n Disallow: /*"); }
           
            
            string? url = await getUrlFromGit(uri);
            await Console.Out.WriteLineAsync(url == null ? "Null" : $"notNull {url}");
            return Redirect(url ?? "https://yaflay.ru/");
        }

        // GET: HomeController/Create
    }
}
