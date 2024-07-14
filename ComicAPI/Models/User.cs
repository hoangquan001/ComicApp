using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ComicApp.Models
{
    public class User
    {
        [Key, Column("id")]
        public int ID { get; set; }

        // [Required, MaxLength(50), Column("username")]
        // public string? Username { get; set; }

        [MaxLength(100), Column("email")]
        public string? Email { get; set; }

        [Required, Column("hashpassword")]
        public string? HashPassword { get; set; }

        [MaxLength(50), Column("firstname")]
        public string? FirstName { get; set; }

        [MaxLength(50), Column("lastname")]
        public string? LastName { get; set; }
        [Column("dob")]
        public DateTime? Dob { get; set; }

        [MaxLength(255), Column("avatar")]
        public string? Avatar { get; set; }

        [Required, Range(0, 1), Column("gender")]
        public int Gender { get; set; } = 0;

        [Required, Range(0, 1), Column("status")]
        public int Status { get; set; } = 1;
        [Column("lastlogin")]
        public DateTime? LastLogin { get; set; }

        [Required, Column("createat")]
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        [Required, Column("updateat")]
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

        [Required, Column("role")]
        [Range(0, 2)]
        public int Role { get; set; }

        [Column("experience")]
        public int Experience { get; set; } = 0;

        [Column("maxim")]
        public string? Maxim { get; set; } = string.Empty;

        [Column("typelevel")]
        public int TypeLevel { get; set; } = 0;
    }

}
