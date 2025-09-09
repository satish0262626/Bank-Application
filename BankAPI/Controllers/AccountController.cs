using System.Security.Claims;
using BankAPI.Data;
using BankAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly BankDbContext _db;
    public AccountController(BankDbContext db) => _db = db;

    private int? GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(id, out var uid)) return uid;
        return null;
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("my-accounts")]
    public async Task<IActionResult> GetMyAccounts()
    {
        var username = User.Identity?.Name;
        var customer = await _db.Customers
            .Include(c => c.User)
            .Include(c => c.Accounts)
            .ThenInclude(a => a.Transactions)
            .FirstOrDefaultAsync(c => c.User.Username == username);

        if (customer is null) return NotFound("Customer not found");

        var dto = customer.Accounts.Select(a => new {
            a.AccountId,
            a.AccountType,
            a.Balance,
            a.CreatedDate
        });

        return Ok(dto);
    }

    public record AmountRequest(int AccountId, decimal Amount);

    [Authorize(Roles = "Customer")]
    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] AmountRequest req)
    {
        var acc = await _db.Accounts.FindAsync(req.AccountId);
        if (acc is null) return NotFound("Account not found");

        acc.Balance += req.Amount;
        _db.Transactions.Add(new Transaction { AccountId = acc.AccountId, Amount = req.Amount, Type = TransactionType.Deposit, Remarks = "Deposit" });
        await _db.SaveChangesAsync();
        return Ok(new { message = "Deposit successful", acc.Balance });
    }

    [Authorize(Roles = "Customer")]
    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] AmountRequest req)
    {
        var acc = await _db.Accounts.FindAsync(req.AccountId);
        if (acc is null) return NotFound("Account not found");
        if (acc.Balance < req.Amount) return BadRequest("Insufficient balance");

        acc.Balance -= req.Amount;
        _db.Transactions.Add(new Transaction { AccountId = acc.AccountId, Amount = req.Amount, Type = TransactionType.Withdraw, Remarks = "Withdraw" });
        await _db.SaveChangesAsync();
        return Ok(new { message = "Withdrawal successful", acc.Balance });
    }

    public record TransferRequest(int FromAccountId, int ToAccountId, decimal Amount);

    [Authorize(Roles = "Customer")]
    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest req)
    {
        if (req.FromAccountId == req.ToAccountId) return BadRequest("Invalid transfer accounts");

        var from = await _db.Accounts.FindAsync(req.FromAccountId);
        var to = await _db.Accounts.FindAsync(req.ToAccountId);
        if (from is null || to is null) return NotFound("Account not found");
        if (from.Balance < req.Amount) return BadRequest("Insufficient funds");

        from.Balance -= req.Amount;
        to.Balance += req.Amount;

        _db.Transactions.Add(new Transaction { AccountId = from.AccountId, Amount = req.Amount, Type = TransactionType.TransferOut, Remarks = $"Transfer to {to.AccountId}" });
        _db.Transactions.Add(new Transaction { AccountId = to.AccountId, Amount = req.Amount, Type = TransactionType.TransferIn, Remarks = $"Transfer from {from.AccountId}" });

        await _db.SaveChangesAsync();
        return Ok(new { message = "Transfer successful" });
    }
}
