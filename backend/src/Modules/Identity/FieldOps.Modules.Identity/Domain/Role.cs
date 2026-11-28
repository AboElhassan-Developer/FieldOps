using Microsoft.AspNetCore.Identity;

namespace FieldOps.Modules.Identity.Domain;

public sealed class Role : IdentityRole<Guid>
{
    public Role()
    {
        Id = Guid.CreateVersion7();
    }
}
