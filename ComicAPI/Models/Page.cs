using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComicApp.Models
{
    public class Page
    {
        [Key]
        [Column("id")]
        public int ID { get; set; } // Primary Key (implicitly set by IDENTITY(1, 1))

        [Column("chapterid")]
        public int ChapterID { get; set; } // Foreign Key

        [Column("pagenumber")]
        public int PageNumber { get; set; }

        [Column("content")]
        public string Content { get; set; } = "";

        [Column("imageurl")]
        public string ImageURL { get; set; } = "";

        // Navigation property
        public virtual Chapter? Chapter { get; set; }
    }

}
