using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrossCutting.EntityFramework;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions context) : base(context) { }

    public DbSet<Question> Questions => Set<Question>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

}
