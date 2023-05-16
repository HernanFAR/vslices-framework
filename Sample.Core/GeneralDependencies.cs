using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Sample.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGeneralDependencies(this IServiceCollection services)
    {
        services
            .AddDbContext<ApplicationDbContext>(opts =>
            {
                const string connectionString = "DataSource=mydatabase.db;";

                var connection = new SqliteConnection(connectionString);

                connection.Open();
                connection.Close();

                connection.Dispose();

                opts.UseSqlite(connectionString);
            });

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddValidatorsFromAssemblyContaining<Core.Anchor>();
        services.AddValidatorsFromAssemblyContaining<Domain.Anchor>();

        return services;
    }
}
