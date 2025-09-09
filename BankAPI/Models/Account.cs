namespace BankAPI.Models;

public enum AccountType { Savings = 1, Current = 2 }

public class Account
{
    public int AccountId { get; set; }
    public int CustomerId { get; set; }
    public AccountType AccountType { get; set; }
    public decimal Balance { get; set; } = 0m;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public Customer Customer { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
