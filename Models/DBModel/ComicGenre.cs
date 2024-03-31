using System.ComponentModel.DataAnnotations;

namespace ComicApp.Models
{
    public class ComicGenre
    {
        // [Key]
        public int comicid { get; set; }
        public Comics? comic { get; set; }
        // [Key]
        public int genreid { get; set; }
        public Genres? genre { get; set; }
    }

}
