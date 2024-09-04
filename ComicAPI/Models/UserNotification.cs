using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ComicApp.Models;

namespace ComicAPI.Models
{
    public class UserNotification
    {
        [Column("userid")]
        public int UserID { get; set; }
        [Column("ntfid")]
        public int NtfID { get; set; }
        [Column("isread")]
        public bool IsRead { get; set; } = false;
        public Notification? notification { get; set; }
    }
}