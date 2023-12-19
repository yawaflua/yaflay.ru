using System.ComponentModel.DataAnnotations.Schema;

namespace yaflay.ru.Models.Tables
{
    public class Redirects
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? uri { get; set; }
        public string? redirectTo { get; set; }
    }
}
