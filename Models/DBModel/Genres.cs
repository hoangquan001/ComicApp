using System.ComponentModel.DataAnnotations;

namespace ComicApp.Models
{
    public class Genres
    {
        public int id { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
    }
}
