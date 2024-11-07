using Microsoft.EntityFrameworkCore;

namespace Obama.Shared;

public interface IDbSeeder<in TContext> where TContext : DbContext
{
    Task SeedAsync(TContext context);
}