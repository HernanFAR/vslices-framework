using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSlices.Core.DataAccess.EntityFramework.UnitTests.Contexts;

public class AppDbContext : DbContext
{
    private readonly string? _connectionString;
    public bool ThrowOnSave { get; set; }

    public AppDbContext() { }

    public AppDbContext(string connectionString)
    {
        _connectionString = new SqlConnectionStringBuilder(connectionString)
        {
            TrustServerCertificate = true
        }.ToString();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_connectionString is not null)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
        else
        {

            optionsBuilder.UseSqlServer();
        }
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
    {
        if (ThrowOnSave)
        {
            throw new DbUpdateConcurrencyException();
        }

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Entity>(e =>
        {
            e.HasKey(e => e.Id);

            e.Property(e => e.Id)
                .ValueGeneratedNever();
        });
        modelBuilder.Entity<DbEntity>(e =>
        {
            e.HasKey(e => e.Id);

            e.Property(e => e.Id)
                .ValueGeneratedNever();
        });
    }

    public class Entity
    {
        public Guid Id { get; private set; }
        public string? Value { get; private set; }

        public Entity(Guid id, string? value)
        {
            Id = id;
            Value = value;
        }
    }

    public class DbEntity
    {
        public Guid Id { get; set; }

        public string? Value { get; set; }

    }
}