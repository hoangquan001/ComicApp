using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ComicApp.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required, MaxLength(50)]
        public string? Username { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [Required]
        public string? HashPassword { get; set; }

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        public DateTime? Dob { get; set; }

        [MaxLength(255)]
        public string? Avatar { get; set; }

        [Required]
        [Range(0, 1)]
        public int Gender { get; set; } = 0;

        [Required]
        [Range(0, 1)]
        public int Status { get; set; } = 1;

        public DateTime? LastLogin { get; set; }

        [Required]
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdateAt { get; set; } = DateTime.Now;

        [Required]
        [Range(0, 2)]
        public int Role { get; set; }
    }

}
