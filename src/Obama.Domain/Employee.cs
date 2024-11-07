namespace Obama.Domain;

public record Employee(string GivenName, string FamilyName, string Mail, int Ags, Guid RoleId) : Auditable
{
    public Employee() : this(string.Empty, string.Empty, string.Empty, 0, Guid.Empty)
    {
    }
    
    public string GivenName { get; set; } = GivenName;
    public string FamilyName { get; set; } = FamilyName;
    public string Mail { get; set; } = Mail;

    public int Ags { get; private set; } = Ags;

    public Guid RoleId { get; set; } = RoleId;
    public Role? Role { get; private set; }
}