using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using yaflay.ru.Models.Tables;

namespace yaflay.ru.Pages
{
    public class BlogModel : PageModel
    {
        public int Id { get; private set; }
        //public Comments[] comments { get; set; } 
        public void OnGet(int? id)
        {
            Id = id ?? 0;
            Page();
            //comments = Startup.dbContext.Comments.Where(k => k.postId == Id).OrderBy(k => k.dateTime).Reverse().ToArray();
        }
        public void OnPost(int? id)
        {
            Id = id ?? 0;
            //comments = Startup.dbContext.Comments.Where(k => k.postId == Id).OrderBy(k => k.dateTime).Reverse().ToArray();
            string userEmail = Request.Form["userEmail"];
            string commentMessage = Request.Form["commentText"];
            if (Id == 0 || commentMessage == null || userEmail == null) { Page(); return; }
            Startup.dbContext.Comments.Add(new Comments()
            {
                Text = commentMessage,
                creatorMail = userEmail,
                dateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                postId = Id
            });
            Startup.dbContext.SaveChanges();
            Page();
            
        }
    }
}
