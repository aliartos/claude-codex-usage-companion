namespace CodexUsageCompanion.Configuration;

public static class OverlayAppearanceOptions
{
    public const int DefaultSpacing = 8;
    public const int DefaultLabelPercentageSpacing = 16;
    public const int MaximumSpacing = 64;

    public static bool IsValidBackgroundColor(string? value)
    {
        var color = value?.Trim();
        return string.IsNullOrEmpty(color) ||
            (color.Length is 7 or 9 && color[0] == '#' &&
             color.AsSpan(1).ContainsAnyExcept("0123456789abcdefABCDEF".AsSpan()) == false);
    }

    public static string NormalizeBackgroundColor(string? value) =>
        IsValidBackgroundColor(value) ? value?.Trim().ToUpperInvariant() ?? string.Empty : string.Empty;

    public static int NormalizeLabelPercentageSpacing(int value) => Math.Clamp(value, 0, 300);

    public static int NormalizeSpacing(int value) => Math.Clamp(value, 0, MaximumSpacing);
}
