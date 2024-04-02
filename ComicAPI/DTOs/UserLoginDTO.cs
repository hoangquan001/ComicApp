
using System.ComponentModel.DataAnnotations;

public class UserLoginDTO
{
    [Required]
    [EmailAddress]
    public string? username { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string? password { get; set; }
}