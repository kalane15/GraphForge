using GraphForge.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphForge.Api.Database;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Graph> Graphs => Set<Graph>();
    public DbSet<Schema> Schemas => Set<Schema>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Graph>()
            .Property(graph => graph.Content)
            .HasColumnType("jsonb")
            .HasDefaultValueSql("""'{"nodes":[],"edges":[]}'::jsonb""");

        modelBuilder.Entity<Schema>()
            .HasIndex(schema => new { schema.ProjectId, schema.Id })
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Login)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(user => user.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Schema>()
            .HasIndex(schema => new { schema.ProjectId, schema.SchemaTypeName })
            .IsUnique();

        modelBuilder.Entity<Project>()
            .Property(project => project.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Project>()
            .Property(project => project.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Graph>()
            .Property(graph => graph.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Graph>()
            .Property(graph => graph.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
