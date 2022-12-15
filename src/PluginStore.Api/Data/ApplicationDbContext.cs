using Microsoft.EntityFrameworkCore;
using PluginStore.Api.Models;

namespace PluginStore.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}