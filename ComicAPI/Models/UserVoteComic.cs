using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ComicAPI.Models
{
    public class UserVoteComic
    {
        [Key]
        [Column("userid")]
        public int UserID { get; set; }
        [Column("comicid")]
        public int ComicID { get; set; }
        [Column("votepoint")]
        public int VotePoint { get; set; }
    }
}