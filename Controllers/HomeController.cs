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
        
        [HttpGet("{uri}")]
        public async Task<IActionResult> FromGitHub(string uri)
        {
            
            string? url = await getUrlFromGit(uri);
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
