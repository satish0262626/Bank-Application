using BankAPI.Data;
using BankAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly BankDbContext _db;
    public AdminController(BankDbContext db) => _db = db;

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _db.Users.Select(u => new { u.UserId, u.Username, u.Role }).ToListAsync();
        return Ok(users);
    }

    public record CreateCustomerRequest(string Username, string Password, string Name, string Email, string Phone);

    [HttpPost("create-customer")]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest req)
    {
        if (await _db.Users.AnyAsync(u => u.Username == req.Username))
            return BadRequest("Username already exists");

        var user = new User { Username = req.Username, PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password), Role = "Customer" };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var customer = new Customer { UserId = user.UserId, Name = req.Name, Email = req.Email, Phone = req.Phone };
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Customer created" });
    }
}
