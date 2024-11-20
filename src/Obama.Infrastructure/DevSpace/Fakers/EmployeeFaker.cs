using Bogus;
using Obama.Domain;

namespace Obama.Infrastructure.DevSpace.Fakers;

public sealed class EmployeeFaker : Faker<Employee>
{
    public EmployeeFaker(IEnumerable<Role> roles)
    {
        CustomInstantiator(faker =>
        {
            var role = faker.PickRandom(roles, 1).Single().Id;

            return new(faker.Person.FirstName, faker.Person.LastName, faker.Person.Email, 0, role);
        });
    }
}