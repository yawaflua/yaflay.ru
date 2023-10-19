using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml.Schema;

namespace yaflay.ru.Новая_папка
{
    [Route("")]
    public class HomeController : Controller
    {
        // GET: HomeController

        private async Task<string> getUrlFromGit(string baseUrl)
        {
            HttpClient client = new();
            string Base64BearerToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(""));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Base64BearerToken);
            HttpResponseMessage getter =  await client.GetAsync("https://raw.githubusercontent.com/YaFlay/yaflay.ru/master/redirect_uris.json?token=GHSAT0AAAAAAB54DYE4TFGPWCCPBHAGGZR4ZJRKIVA");
            JsonNode allFile = JsonNode.Parse(await getter.Content.ReadAsStringAsync());
            return (string?)allFile[baseUrl];
        }
        // GET: HomeController/Details/5
        [HttpGet]
        public async Task<IActionResult> fromGitHub()
        {
            string? url = await getUrlFromGit(HttpContext.Request.Path.Value);

            return Redirect(url != null ? url : "yaflay.ru");
        }

        // GET: HomeController/Create
    }
}
