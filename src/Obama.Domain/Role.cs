namespace Obama.Domain;

public record Role(string Name, bool Enabled) : Auditable
{
    public Role() : this(string.Empty, false)
    {
    }

    public string Name { get; set; } = Name;
    public bool Enabled { get; set; } = Enabled;

    public HashSet<Employee> Employees { get; private set; } = [];
}