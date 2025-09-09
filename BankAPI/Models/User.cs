using System.ComponentModel.DataAnnotations;

namespace BankAPI.Models;

public class User
{
    public int UserId { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    [Required, MaxLength(20)]
    public string Role { get; set; } = "Customer"; // Admin, Customer, Teller
}
