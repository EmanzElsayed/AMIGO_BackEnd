using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Amigo.Application.Helpers;

public static class SlugHelper
{
    public static string Normalize(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var lower = text.Trim().ToLowerInvariant();
        var sb = new StringBuilder(lower.Length);
        foreach (var ch in lower.Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }

        var s = sb.ToString().Normalize(NormalizationForm.FormC);
        s = s.Replace(' ', '-');
        return s.Trim('-');
    }

    public static string ToUrlSlug(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "tour";

        var lower = text.Trim().ToLowerInvariant();
        var sb = new StringBuilder(lower.Length);
        foreach (var ch in lower.Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }

        var s = sb.ToString().Normalize(NormalizationForm.FormC);
        s = Regex.Replace(s, @"[^a-z0-9]+", "-");
        s = Regex.Replace(s, @"-+", "-").Trim('-');
        return string.IsNullOrEmpty(s) ? "tour" : s;
    }

    public static bool MatchesName(string? destinationName, string slug)
    {
        if (string.IsNullOrEmpty(destinationName) || string.IsNullOrEmpty(slug))
            return false;
        return string.Equals(Normalize(destinationName), Normalize(slug), StringComparison.OrdinalIgnoreCase);
    }


}
