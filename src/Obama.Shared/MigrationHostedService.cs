using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static System.Threading.Tasks.Task;

namespace Obama.Shared;

internal class MigrationHostedService<TContext>(IServiceProvider serviceProvider, Func<TContext, IServiceProvider, Task> seeder) : BackgroundService where TContext : DbContext
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var provider = scope.ServiceProvider;

        var logger = provider.GetRequiredService<ILogger<TContext>>();
        var context = provider.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

            async Task InvokeSeederAsync()
            {
                await context.Database.MigrateAsync(cancellationToken);
                await seeder(context, provider);
            }

            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(InvokeSeederAsync);

            logger.LogInformation("Migration of the database associated with context {DbContextName} has completed", typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}",
                typeof(TContext).Name);
            throw;
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return CompletedTask;
    }
}