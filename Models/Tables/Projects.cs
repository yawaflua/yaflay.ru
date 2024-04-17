using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.yawaflua.ru.Models.Tables
{
    [Table("Projects")]
    public class Projects
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string image { get; set; }
    }
}
