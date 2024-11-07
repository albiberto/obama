using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Obama.Infrastructure.DevSpace;
using Obama.Shared;

namespace Obama.Infrastructure;

public static class Bootstrapper
{
    public static void AddDevSeeder(this IServiceCollection services)
    {
    }

    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("ObamaConnection");
        if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("Connection string not found!");

        services.AddDbContext<ObamaContext>(options => options.UseSqlServer(connectionString));

        if (environment.IsDevelopment())
        {
            services.AddMigration<ObamaContext, ObamaDevContextSeeder>();
            return;
        }

        services.AddMigration<ObamaContext>();
    }
}