using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace yaflay.ru.Pages
{
    public class AdminPanelModel : PageModel
    {
        public string? type = null;
        public void OnGet(string? type)
        {
            this.type = type;
        }
    }
}
