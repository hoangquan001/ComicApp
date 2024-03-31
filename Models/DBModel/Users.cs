using System.ComponentModel.DataAnnotations;

namespace ComicApp.Models
{
    public class Users
    {
        [Key]
        public int id { get; set; }
        public string username { get; set; } = "";
        public string hash_password { get; set; } = "";
        [Required]
        [EmailAddress]
        public string email { get; set; } = "";
    }

}
