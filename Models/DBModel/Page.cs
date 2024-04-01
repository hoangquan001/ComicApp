using System.ComponentModel.DataAnnotations;

namespace ComicApp.Models
{
    public class Page
    {
        public int ID { get; set; } // Primary Key (implicitly set by IDENTITY(1, 1))
        public int ChapterID { get; set; } // Foreign Key
        public int PageNumber { get; set; }
        public string Content { get; set; } = "";
        public string ImageURL { get; set; } = "";
    }

}
