using Microsoft.EntityFrameworkCore;
using Obama.Domain;
using Obama.Infrastructure.DevSpace.Fakers;
using Obama.Shared;

namespace Obama.Infrastructure.DevSpace;

public class ObamaDevContextSeeder : IDbSeeder<ObamaContext>
{
    public async Task SeedAsync(ObamaContext context)
    {
        await SeedRolesAsync(context);
        await SeedEmployeesAsync(context);
        await SeedForAperiTechAsync(context);
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
        var employees = new EmployeeFaker(roles).Generate(100);

        await context.Employees.AddRangeAsync(employees);
        await context.SaveChangesAsync();
    }

    private static async Task SeedForAperiTechAsync(ObamaContext context)
    {
        const string aperiTeacher = "AperiTeacher";

        var role = context.Roles.SingleOrDefault(role => role.Name == aperiTeacher);
        if (role is { }) return;

        var newRole = new Role
        {
            Id = new("00000000-0000-0000-0000-000000000001"),
            Name = aperiTeacher,
            Enabled = true
        };

        var teacherRole = await context.Roles.AddAsync(newRole);

        HashSet<Employee> teachers =
        [
            new()
            {
                Id = new("00000000-0000-0000-0000-000000000001"),
                GivenName = "Alberto",
                FamilyName = "Viezzi",
                Mail = "alberto.viezzi@euris.it",
                RoleId = teacherRole.Entity.Id
            },

            new()
            {
                Id = new("00000000-0000-0000-0000-000000000002"),
                GivenName = "Matteo",
                FamilyName = "Perazza",
                Mail = "matteo.perazza@euris.it",
                RoleId = teacherRole.Entity.Id
            }
        ];

        await context.Employees.AddRangeAsync(teachers);
        await context.SaveChangesAsync();
    }
}