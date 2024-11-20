using Bogus;
using Obama.Domain;

namespace Obama.Infrastructure.DevSpace.Fakers;

public class RoleFaker : Faker<Role>
{
    public RoleFaker()
    {
        CustomInstantiator(faker => new(faker.Name.JobType(), faker.Random.Bool(.75f)));
    }
}