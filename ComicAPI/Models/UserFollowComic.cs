using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ComicApp.Models
{
    public class UserFollowComic
    {
        [Column("comicid")]
        public int ComicID { get; set; }

        [Column("userid")]
        public int UserID { get; set; }

        [Column("createat")]
        public DateTime CreateAt { get; set; }

        public Comic? comic;
    }

}
