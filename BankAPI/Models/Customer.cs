//namespace BankAPI.Models
//{
//    public class Customer
//    {
//    }
//}

namespace BankAPI.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public int UserId { get; set; }            // FK to Users
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}

