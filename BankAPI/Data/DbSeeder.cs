using BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Data;

public class DbSeeder
{
    public static async Task SeedAsync(BankDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        if (!await db.Users.AnyAsync())
        {
            var admin = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin"
            };
            var custUser = new User
            {
                Username = "john",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("john123"),
                Role = "Customer"
            };

            db.Users.AddRange(admin, custUser);
            await db.SaveChangesAsync();

            var customer = new Customer
            {
                UserId = custUser.UserId,
                Name = "John Doe",
                Email = "john@bank.com",
                Phone = "9999999999"
            };
            db.Customers.Add(customer);
            await db.SaveChangesAsync();

            db.Accounts.Add(new Account { CustomerId = customer.CustomerId, AccountType = AccountType.Savings, Balance = 5000m });
            await db.SaveChangesAsync();
        }
    }
}
