using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

namespace yaflay.ru.Models.Tables
{
    [Table("Comments", Schema = "public")]
    public class Comments
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public long dateTime { get; set; }
        public string Text { get; set; }
        public string creatorMail { get; set; }
        public int postId { get; set; }

    }
}
