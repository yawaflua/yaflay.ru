using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace yawaflua.ru.Models.Tables
{
    [Table("Blogs", Schema = "public")]
    public class Blogs
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Annotation { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime dateTime { get; set; }
        public string authorId { get; set; }
        public string authorNickname { get; set;  }
    }

    public class Author
    {
        public int Id { get; set; }
        public ulong discordId { get; set; }
        public string discordNickName { get; set; }
    }
}
