using GraphForge.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphForge.Api.Database;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}
