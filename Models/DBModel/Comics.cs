using System.ComponentModel.DataAnnotations;

namespace ComicApp.Models
{
    public class Comics
    {
        [Key]
        public int id { get; set; }
        public string? title { get; set; }
        public string? author { get; set; }
        public string? description { get; set; }
        public string? cover_image { get; set; }
        // public List<Chapters> chapters { get; set; } = [];
    }
}
