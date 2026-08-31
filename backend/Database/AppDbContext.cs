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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(user => user.Id);

            entity.Property(user => user.Id).HasColumnName("id");
            entity.Property(user => user.Login).HasColumnName("login");
            entity.Property(user => user.PasswordHash).HasColumnName("password_hash");
            entity.Property(user => user.CreatedAt).HasColumnName("created_at");
        });
    }
}
