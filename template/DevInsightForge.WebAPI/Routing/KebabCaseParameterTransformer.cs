using Microsoft.AspNetCore.Routing;
using System.Text.RegularExpressions;

namespace DevInsightForge.WebAPI.Routing;

public sealed class KebabCaseParameterTransformer : IOutboundParameterTransformer
{
    private static readonly Regex SplitWordsRegex = new("([a-z0-9])([A-Z])", RegexOptions.Compiled);

    public string? TransformOutbound(object? value)
    {
        if (value is null)
        {
            return null;
        }

        var valueAsString = value.ToString();
        if (string.IsNullOrWhiteSpace(valueAsString))
        {
            return valueAsString;
        }

        return SplitWordsRegex.Replace(valueAsString, "$1-$2").ToLowerInvariant();
    }
}
