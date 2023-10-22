using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;
using System.Text.Json.Nodes;
using yaflay.ru.Pages;

namespace yaflay.ru.Новая_папка
{
    [Route("")]
    public class HomeController : Controller
    {
        // GET: HomeController

        private async Task<string?> getUrlFromGit(string baseUrl)
        {
            try
            {
                HttpClient client = new();
                HttpResponseMessage getter = await client.GetAsync("https://raw.githubusercontent.com/YaFlay/yaflay.ru/master/redirect_uris.json");
                JsonNodeOptions jsonNodeOptions = new ();
                JsonDocumentOptions jsonDocumentOptions = new()
                {
                    AllowTrailingCommas = true
                };
                JsonNode? allFile = JsonNode.Parse(await getter.Content.ReadAsStringAsync(),
                                                 nodeOptions: jsonNodeOptions,
                                                 documentOptions: jsonDocumentOptions);
                ;
                return (string?)allFile[baseUrl];
            }
            catch (Exception except)
            {
                await Console.Out.WriteLineAsync(except.ToString());
                return null;
            }
        }
        
        [HttpGet("{uri}")]
        public async Task<IActionResult> FromGitHub(string uri)
        {
            
            string? url = await getUrlFromGit(uri);
            await Console.Out.WriteLineAsync($"New connected user: {HttpContext.Connection.RemoteIpAddress}");
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
