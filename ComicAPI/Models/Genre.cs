using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComicApp.Models
{
    public class Genre
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }
        [Column("title")]
        public string Title { get; set; } = string.Empty;
        [Column("slug")]
        public string Slug { get; set; } = string.Empty;
        // [Column("description")]
        // public string Description { get; set; } = string.Empty;
    }
}
