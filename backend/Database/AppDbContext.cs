using GraphForge.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphForge.Api.Database;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Graph> Graphs => Set<Graph>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //This code is redundant because of the EF conventions, but I will leave it here for clarity

        modelBuilder.Entity<Project>()
            .HasOne(project => project.Owner)
            .WithMany()
            .HasForeignKey(project => project.OwnerId);

        modelBuilder.Entity<Graph>()
            .HasOne(graph => graph.Project)
            .WithMany(project => project.Graphs)
            .HasForeignKey(graph => graph.ProjectId);
    }
}
