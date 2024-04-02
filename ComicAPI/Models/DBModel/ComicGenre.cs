using System.ComponentModel.DataAnnotations;

namespace ComicApp.Models
{
    public class ComicGenre
    {
        // [Key]
        public int ComicID { get; set; }
        public Comic? Comic { get; set; }
        // [Key]
        public int GenreID { get; set; }
        public Genre? Genre { get; set; }
    }

}
