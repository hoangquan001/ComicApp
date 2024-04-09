using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ComicApp.Data;

namespace ComicApp.Models
{
    public class Comic
    {
        [Key]
        public int ID { get; set; } // Primary Key (implicitly set by IDENTITY(1, 1))
        public string Title { get; set; } = "";
        public string URL { get; set; } = "";
        public string ?Author { get; set; }
        public string ?Description { get; set; }
        public string ?CoverImage { get; set; } 
        public int Status { get; set; }  = 0;   // Enforced by data annotation check constraint
        public int Rating { get; set; } =10;// Enforced by data annotation check constraint
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
        public List<Genre> genres { get; set; } = [];
        public List<Chapter> Chapters { get; set; } = [];
    }
   
}
