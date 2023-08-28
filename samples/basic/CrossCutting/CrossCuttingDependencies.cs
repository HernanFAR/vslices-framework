using CrossCutting.EntityFramework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CrossCuttingDependencies
{
    public static IServiceCollection AddCrossCuttingDependencies(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(builder =>
        {
            using (var connection = new SqliteConnection("Data Source=tests.db"))
            {
                connection.Open();
            }

            builder.UseSqlite("Data Source=tests.db");

        });


        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddLoggingBehavior();
        services.AddFluentValidationBehavior();

        return services;
    }
}
