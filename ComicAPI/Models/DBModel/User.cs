using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ComicApp.Models
{
    // CREATE TABLE USER (
    //     ID INT NOT NULL IDENTITY(1,1),
    //     Username NVARCHAR(50) UNIQUE,
    //     Email NVARCHAR(100),
    //     HashPassword NVARCHAR(255),
    //     Role int NOT NULL CHECK (Role IN(0,1,2)),
    // 	CONSTRAINT PK_USER PRIMARY KEY (ID)
    // );
    public class User
    {
        [Key]
        public int ID { get; set; }
        public string Username { get; set; } = "";
        public string HashPassword { get; set; } = "";
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}
