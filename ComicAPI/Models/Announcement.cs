// CREATE Table announcement
// (
//     id serial primary key,
//     title text,
//     content text,
//     createat timestamp default now(),
//     updateat timestamp default now(),
//     isvisible boolean DEFAULT true,
//     applyfrom timestamp default now(),
//     applyto timestamp default now()
// )

using System.ComponentModel.DataAnnotations.Schema;

namespace ComicAPI.Models
{
    public class Announcement
    {
        [Column("id")]
        public int ID { get; set; }
        [Column("title")]
        public string Title { get; set; } = "";
        [Column("content")]
        public string Content { get; set; } = "";
        [Column("createat")]
        public DateTime CreateAt { get; set; }
        [Column("updateat")]
        public DateTime UpdateAt { get; set; }
        [Column("isvisible")]
        public bool IsVisible { get; set; }
        [Column("applyfrom")]
        public DateTime ApplyFrom { get; set; }
        [Column("applyto")]
        public DateTime ApplyTo { get; set; }
    }
}