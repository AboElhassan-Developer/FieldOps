using System.Globalization;
using System.Text.RegularExpressions;

namespace FieldOps.Api.Http;

/// <summary>Turns route tokens such as <c>[controller]</c> into kebab case: <c>WorkOrders</c> becomes <c>work-orders</c>.</summary>
internal sealed partial class KebabCaseParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value) =>
        value is null
            ? null
            : WordBoundary().Replace(Convert.ToString(value, CultureInfo.InvariantCulture)!, "$1-$2").ToLowerInvariant();

    [GeneratedRegex("([a-z0-9])([A-Z])")]
    private static partial Regex WordBoundary();
}
