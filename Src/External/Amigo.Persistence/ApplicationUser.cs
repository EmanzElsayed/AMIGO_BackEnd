using Amigo.Domain.Enum;
using Microsoft.AspNetCore.Identity;

namespace Amigo.Persistence;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string? Gender { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? Address { get; set; }
    public Language? PreferredLanguage { get; set; }
    public bool IsInactive { get; set; }
}

