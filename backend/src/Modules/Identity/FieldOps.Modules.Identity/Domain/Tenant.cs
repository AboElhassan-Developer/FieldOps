namespace FieldOps.Modules.Identity.Domain;

public sealed class Tenant
{
    private Tenant()
    {
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public string Name { get; private set; } = string.Empty;

    public TenantStatus Status { get; private set; } = TenantStatus.Active;

    /// <summary>IANA time zone id, e.g. <c>Africa/Cairo</c>.</summary>
    public string TimeZone { get; private set; } = string.Empty;

    /// <summary>Language code: <c>en</c> or <c>ar</c>.</summary>
    public string DefaultLanguage { get; private set; } = string.Empty;

    /// <summary>ISO 4217 currency code, e.g. <c>EGP</c>.</summary>
    public string Currency { get; private set; } = string.Empty;

    /// <summary>Tax rate as a fraction, e.g. <c>0.1400</c> for 14%.</summary>
    public decimal TaxRate { get; private set; }

    public string InvoicePrefix { get; private set; } = string.Empty;
}
