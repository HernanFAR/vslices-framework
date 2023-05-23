using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using VSlices.Core.DataAccess.EntityFramework.UnitTests.Contexts;
using VSlices.Core.DataAccess.EntityFramework.UnitTests.Factories;

namespace VSlices.Core.DataAccess.EntityFramework.UnitTests;

public class Fixture : IAsyncLifetime
{
    public MsSqlContainer DbContainer { get; } = DatabaseFactory.BuildContainer();
    public AppDbContext Context { get; set; } = default!;

    public async Task InitializeAsync()
    {
        await DbContainer.StartAsync();

        Context = new AppDbContext(DbContainer.GetConnectionString());

        await Context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContainer.DisposeAsync();
        await Context.DisposeAsync();
    }
}
