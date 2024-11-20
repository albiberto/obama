using System.ComponentModel.DataAnnotations;

namespace Obama.Domain;

public record Auditable : Entity
{
    [Required] public DateTime CreatedAt { get; set; }

    [Required] public DateTime UpdatedAt { get; set; }
}