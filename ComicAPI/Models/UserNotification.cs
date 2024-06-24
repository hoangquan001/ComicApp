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
        [Key]
        [Column("id")]
        public int ID { get; set; }
        [Column("userid")]
        public int UserID { get; set; }
        [Column("comicid")]
        public int ComicID { get; set; }
        [Column("notificationcontent")]
        public string NotificationContent { get; set; } = "";
        [Column("notificationtimestamp")]
        public DateTime NotificationTimestamp { get; set; } = DateTime.UtcNow;
        [Column("isread")]
        public Boolean IsRead { get; set; } = false;

        public Comic? comic { get; set; }
        public User? user;

    }
}