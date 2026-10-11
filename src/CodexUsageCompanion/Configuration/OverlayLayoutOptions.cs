namespace CodexUsageCompanion.Configuration;

public static class OverlayLayoutOptions
{
    public const double DefaultFontScale = 1d;
    public const double MinimumFontScale = 0.8d;
    public const double MaximumFontScale = 1.5d;
    public const double FiveHourCardHeight = 36d;
    public const double WeeklyCardHeight = 56d;
    public const double MaximumCardHeight = 320d;

    public static double NormalizeFontScale(double value) =>
        double.IsFinite(value)
            ? Math.Round(Math.Clamp(value, MinimumFontScale, MaximumFontScale), 2)
            : DefaultFontScale;

    public static double NormalizeCardHeight(double value, bool weekly) =>
        double.IsFinite(value)
            ? Math.Round(Math.Clamp(
                value,
                weekly ? WeeklyCardHeight : FiveHourCardHeight,
                MaximumCardHeight))
            : weekly ? WeeklyCardHeight : FiveHourCardHeight;

    public static double MinimumCardHeight(bool weekly, double fontScale) =>
        Math.Max(FiveHourCardHeight, Math.Ceiling((weekly ? WeeklyCardHeight : FiveHourCardHeight) * fontScale));
}
