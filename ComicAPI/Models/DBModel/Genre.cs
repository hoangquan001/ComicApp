using System.ComponentModel.DataAnnotations;

namespace ComicApp.Models
{
    public class Genre
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
