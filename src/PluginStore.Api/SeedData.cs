using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PluginStore.Api.Data;
using PluginStore.Api.Models;

namespace PluginStore.Api;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        context.Database.Migrate();

        var admin = context.Users.FirstOrDefaultAsync(u => u.Email == "admin@example.com").Result;

        if (admin == null)
        {
            using var hmac = new HMACSHA512();
            admin = new User
            {
                Name = "Admin",
                Email = "admin@example.com",
                Role = Roles.Administrator,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("admin")),
                PasswordSalt = hmac.Key
            };
            context.Users.Add(admin);
            context.SaveChanges();
            Console.WriteLine("Admin created");
        }
        else
        {
            Console.WriteLine("Admin already exists");
        }
    }
}