using System.Globalization;
using System.Text;

namespace Vmf.Publisher.Domain;

/// <summary>Defines the canonical representation of an explicit block identifier.</summary>
internal static class ExplicitBlockIdRules
{
    internal const int MaximumLengthInUnicodeScalars = 128;

    internal static string? NormalizeOptional(string? value, string parameterName)
    {
        if (value is null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("An explicit block identifier must not be empty.", parameterName);
        }

        var normalized = value.Normalize(NormalizationForm.FormC);
        var scalarCount = 0;
        foreach (var rune in normalized.EnumerateRunes())
        {
            if (scalarCount == 0)
            {
                if (!Rune.IsLetter(rune) && rune.Value != '_')
                {
                    throw Invalid(parameterName);
                }
            }
            else if (!IsContinuation(rune))
            {
                throw Invalid(parameterName);
            }

            scalarCount++;
            if (scalarCount > MaximumLengthInUnicodeScalars)
            {
                throw new ArgumentException(
                    $"An explicit block identifier must not exceed {MaximumLengthInUnicodeScalars} Unicode scalar values.",
                    parameterName);
            }
        }

        return normalized;
    }

    private static bool IsContinuation(Rune rune)
    {
        if (Rune.IsLetterOrDigit(rune) || rune.Value is '_' or '-' or '.' or ':')
        {
            return true;
        }

        return Rune.GetUnicodeCategory(rune) is
            UnicodeCategory.NonSpacingMark or
            UnicodeCategory.SpacingCombiningMark;
    }

    private static ArgumentException Invalid(string parameterName) => new(
        "An explicit block identifier must start with a Unicode letter or underscore and " +
        "continue with Unicode letters, digits, combining marks, underscore, hyphen, period, or colon.",
        parameterName);
}
