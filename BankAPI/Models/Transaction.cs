namespace BankAPI.Models;

public enum TransactionType { Deposit = 1, Withdraw = 2, TransferOut = 3, TransferIn = 4 }

public class Transaction
{
    public int TransactionId { get; set; }
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Remarks { get; set; }
    public Account Account { get; set; } = null!;
}

