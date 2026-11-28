using Microsoft.AspNetCore.Identity;

namespace FieldOps.Modules.Identity.Domain;

public sealed class User : IdentityUser<Guid>
{
    public User()
    {
        Id = Guid.CreateVersion7();
    }

    /// <summary>Owning tenant; null only for platform administrators.</summary>
    public Guid? TenantId { get; private set; }

    public string FullName { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;

    /// <summary>Language code: <c>en</c> or <c>ar</c>.</summary>
    public string PreferredLanguage { get; private set; } = string.Empty;

    public bool MustChangePassword { get; private set; }
}
