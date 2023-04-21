using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> c) : 
        base(c) { }

    public DbSet<Question> Questions => Set<Question>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
