using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static System.Threading.Tasks.Task;

namespace Obama.Shared;

public static class Extensions
{
    public static void AddMigration<TContext, TDbSeeder>(this IServiceCollection services) where TContext : DbContext
        where TDbSeeder : class, IDbSeeder<TContext>
    {
        services.AddScoped<IDbSeeder<TContext>, TDbSeeder>();
        services.AddMigration<TContext>((context, provider) => provider.GetRequiredService<IDbSeeder<TContext>>().SeedAsync(context));
    }

    public static void AddMigration<TContext>(this IServiceCollection services, Func<TContext, IServiceProvider, Task>? seeder = default)
        where TContext : DbContext
    {
        seeder ??= (_, _) => CompletedTask;
        services.AddHostedService(provider => new MigrationHostedService<TContext>(provider, seeder));
    }
}