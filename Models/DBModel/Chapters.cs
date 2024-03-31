using System.ComponentModel.DataAnnotations;

namespace ComicApp.Models
{
    public class Chapters
    {
        [Key]
        public int id { get; set; }
        public int comicid { get; set; }
        public Comics comic { get; set; } = null!;
        public int chapter_number { get; set; }
        public string title { get; set; } = "";
        public string content { get; set; } = "";

    }

}
