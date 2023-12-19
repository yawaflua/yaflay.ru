using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace yaflay.ru.Pages
{
    public class AuthorizeModel : PageModel
    {
        public string code;
        public void OnGet(string code)
        {
            
           
            this.code = code;
            Page();
            
        }
    }
}
