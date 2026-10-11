using CodexUsageCompanion.Configuration;
using Xunit;

namespace CodexUsageCompanion.Tests;

public sealed class SettingsSectionStateTests
{
    [Fact]
    public void MissingSectionPreferencesKeepExistingExpandedDefault()
    {
        var sections = new CompanionSettings().SettingsSections;
        foreach (var key in new[] { "appearance", "dateTime", "logging", "notifications", "usage", "window" })
            Assert.True(sections.IsExpanded(key));
    }

    [Fact]
    public void MixedSectionStatesPersistWithoutChangingOtherPreferences()
    {
        var path = Path.Combine(Path.GetTempPath(), $"settings-sections-{Guid.NewGuid():N}.json");
        try
        {
            var saved = CompanionSettingsStore.Save(new CompanionSettings
            {
                BackgroundColor = "#123456",
                EnableClaudeUsage = false
            }, path);
            var sections = new SettingsSectionState
            {
                Appearance = false,
                DateTime = true,
                Logging = false,
                Notifications = true,
                Usage = false,
                Window = true
            };
            CompanionSettingsStore.Save(saved with { SettingsSections = sections }, path);
            var restored = CompanionSettingsStore.Load(path);
            Assert.Equal(sections, restored.SettingsSections);
            Assert.False(restored.EnableClaudeUsage);
            Assert.Equal("#123456", restored.BackgroundColor);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void NullSectionPreferencesFallBackToExpandedSections()
    {
        var normalized = CompanionSettingsStore.Normalize(new CompanionSettings { SettingsSections = null! });
        Assert.Equal(new SettingsSectionState(), normalized.SettingsSections);
    }
}
