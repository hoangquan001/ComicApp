using System.ComponentModel.DataAnnotations;
using ComicApp.Data;

namespace ComicApp.Models
{
    public class Comics
    {
        public Comics()
        {

        }
        private Comics(DataContext context)
        {
            Context = context;
        }
        private DataContext Context { get; set; }
        [Key]
        public int id { get; set; }
        public string? title { get; set; }
        public string? author { get; set; }
        public string? description { get; set; }
        public string? cover_image { get; set; }

        public List<Genres> genres { get; set; } = [];

        public int count => Context.ComicGenre.Count(i => i.comicid == id);
    }
}
