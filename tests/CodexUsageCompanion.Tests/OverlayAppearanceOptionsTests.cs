using CodexUsageCompanion.Configuration;
using Xunit;

namespace CodexUsageCompanion.Tests;

public sealed class OverlayAppearanceOptionsTests
{
    [Theory]
    [InlineData(null, true, "")]
    [InlineData("", true, "")]
    [InlineData(" #12abef ", true, "#12ABEF")]
    [InlineData("#8012abef", true, "#8012ABEF")]
    [InlineData("#123", false, "")]
    [InlineData("#12345G", false, "")]
    [InlineData("red", false, "")]
    public void ValidatesAndNormalizesColor(string? input, bool valid, string expected)
    {
        Assert.Equal(valid, OverlayAppearanceOptions.IsValidBackgroundColor(input));
        Assert.Equal(expected, OverlayAppearanceOptions.NormalizeBackgroundColor(input));
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(24, 24)]
    [InlineData(100, 64)]
    public void NormalizationBoundsSpacing(int input, int expected)
    {
        var normalized = CompanionSettingsStore.Normalize(new CompanionSettings
        {
            BackgroundColor = "invalid",
            PercentageTimeSpacing = input,
            LabelPercentageSpacing = input
        });
        Assert.Equal(expected, normalized.PercentageTimeSpacing);
        Assert.Equal(Math.Clamp(input, 0, 300), normalized.LabelPercentageSpacing);
        Assert.Equal("", normalized.BackgroundColor);
    }

    [Fact]
    public void AppearanceSettingsSurviveSaveAndLoad()
    {
        var path = Path.Combine(Path.GetTempPath(), $"appearance-{Guid.NewGuid():N}.json");
        try
        {
            CompanionSettingsStore.Save(new CompanionSettings
            {
                BackgroundColor = "#802b2827",
                PercentageTimeSpacing = 24,
                LabelPercentageSpacing = 12
            }, path);
            var loaded = CompanionSettingsStore.Load(path);
            Assert.Equal("#802B2827", loaded.BackgroundColor);
            Assert.Equal(24, loaded.PercentageTimeSpacing);
            Assert.Equal(12, loaded.LabelPercentageSpacing);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
