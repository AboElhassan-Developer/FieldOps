using FieldOps.Modules.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FieldOps.Modules.Identity.Infrastructure.Persistence.Configurations;

internal sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants", table =>
            table.HasCheckConstraint("CK_Tenants_TaxRate", "[TaxRate] >= 0 AND [TaxRate] < 1"));

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();

        builder.Property(t => t.Name).HasMaxLength(200).IsRequired();
        builder.Property(t => t.TimeZone).HasMaxLength(64).IsRequired();
        builder.Property(t => t.DefaultLanguage).HasMaxLength(5).IsRequired();
        builder.Property(t => t.Currency).HasMaxLength(3).IsFixedLength().IsRequired();
        builder.Property(t => t.TaxRate).HasPrecision(5, 4);
        builder.Property(t => t.InvoicePrefix).HasMaxLength(10).IsRequired();
    }
}
