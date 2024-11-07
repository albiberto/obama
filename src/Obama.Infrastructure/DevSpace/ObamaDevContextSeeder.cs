using Microsoft.EntityFrameworkCore;
using Obama.Infrastructure.DevSpace.Fakers;
using Obama.Shared;

namespace Obama.Infrastructure.DevSpace;

public class ObamaDevContextSeeder : IDbSeeder<ObamaContext>
{
    public async Task SeedAsync(ObamaContext context)
    {
        await SeedRolesAsync(context);
        await SeedEmployeesAsync(context);
    }

    private static async Task SeedRolesAsync(ObamaContext context)
    {
        if (context.Roles.Any()) return;

        var roles = new RoleFaker().Generate(4);

        await context.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }

    private static async Task SeedEmployeesAsync(ObamaContext context)
    {
        if (context.Employees.Any()) return;

        var roles = await context.Roles.Where(role => role.Enabled).ToListAsync();
        var employee = new EmployeeFaker(roles).Generate(50);

        await context.Employees.AddRangeAsync(employee);
        await context.SaveChangesAsync();
    }
}