using BankAPI.Data;
using BankAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly BankDbContext _db;
    private readonly JwtService _jwt;

    public AuthController(BankDbContext db, JwtService jwt) { _db = db; _jwt = jwt; }

    public record LoginRequest(string Username, string Password);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == req.Username);
        if (user is null) return Unauthorized("Invalid username or password");

        var ok = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
        if (!ok) return Unauthorized("Invalid username or password");

        var token = _jwt.GenerateToken(user.UserId, user.Username, user.Role);
        return Ok(new { token, username = user.Username, role = user.Role });
    }
}
