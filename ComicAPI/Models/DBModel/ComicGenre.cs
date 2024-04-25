using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComicApp.Models
{
    public class ComicGenre
    {
        // [Key]
        [Column("comicid")]
        public int ComicID { get; set; }
        public Comic? Comic { get; set; }
        // [Key]
        [Column("genreid")]
        public int GenreID { get; set; }
        public Genre? Genre { get; set; }
    }

}
