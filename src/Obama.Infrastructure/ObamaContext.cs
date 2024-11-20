using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Obama.Domain;

namespace Obama.Infrastructure;

public class ObamaContext(DbContextOptions<ObamaContext> options, IInterceptor interceptor) : DbContext(options)
{
    public DbSet<Employee> Employees { get; init; }
    public DbSet<Role> Roles { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>()
            .Property(e => e.Bonus)
            .HasColumnType("money");
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(interceptor);

        base.OnConfiguring(optionsBuilder);
    }
}