using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.VisualTree;
using CodexUsageCompanion.Configuration;
using CodexUsageCompanion.Localization;
using CodexUsageCompanion.Lifecycle;
using CodexUsageCompanion.RateLimits;
using CodexUsageCompanion.Ui;
using Xunit;

[assembly: AvaloniaTestApplication(typeof(CodexUsageCompanion.Tests.OverlayTestApplication))]

namespace CodexUsageCompanion.Tests;

public static class OverlayTestApplication
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<Application>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
}

public sealed class OverlayMergeTests
{
    private static readonly HeadlessUnitTestSession Session =
        HeadlessUnitTestSession.StartNew(typeof(OverlayTestApplication));
    [Fact]
    public Task UsageRefreshPreservesResizedHeightAndIndependentProviderError() => Session.Dispatch(() =>
    {
        var window = new UsageOverlayWindow();
        window.Show();
        window.Height = 600;
        window.Width = 700;
        window.SetStatus(null, "Claude refresh failed", UsageProvider.Claude);
        window.UpdateUsage(UsageProvider.Codex, null);
        window.SetStatus(DateTimeOffset.UtcNow, null, UsageProvider.Codex);

        Assert.True(window.CanResize);
        Assert.Equal(600, window.Height);
        Assert.Equal(700, window.Width);
        Assert.Contains(window.GetVisualDescendants().OfType<TextBlock>(),
            text => text.Text == "Claude refresh failed");
        window.Close();
    }, CancellationToken.None);

    [Fact]
    public Task FontAndCardSizingCoexistWithProviderHeadingsAndAppearance() => Session.Dispatch(() =>
    {
        var settings = new CompanionSettings
        {
            FontScale = 1.5,
            ClaudeSessionCardHeight = 200,
            BackgroundColor = "#123456",
            LabelPercentageSpacing = 24,
            PercentageTimeSpacing = 12
        };
        var window = new UsageOverlayWindow(settings);
        window.Show();
        var texts = window.GetVisualDescendants().OfType<TextBlock>().ToArray();

        Assert.Contains(texts, text => text.Text == "Claude" && text.FontSize == 31.5);
        Assert.Contains(texts, text => text.Text == "Codex" && text.FontSize == 31.5);
        Assert.Contains(window.GetVisualDescendants().OfType<Border>(), border => border.Height == 200);
        Assert.True(window.Height >= 200);

        window.ApplySettings(settings with { EnableClaudeUsage = false }, UiText.For(UiLanguage.English));
        Assert.DoesNotContain(window.GetVisualDescendants().OfType<TextBlock>(),
            text => text.Text == "Claude" && text.IsEffectivelyVisible);
        window.Close();
    }, CancellationToken.None);

    [Fact]
    public Task CardResizeHandleUpdatesHeightAndNotifiesPersistence() => Session.Dispatch(() =>
    {
        var window = new UsageOverlayWindow();
        window.Show();
        UsageCardKind? changedKind = null;
        double changedHeight = 0;
        window.CardHeightChanged += (kind, height) => { changedKind = kind; changedHeight = height; };
        var handle = window.GetVisualDescendants().OfType<Border>()
            .First(border => border.Focusable && border.Height == 12);
        handle.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Down });

        Assert.Equal(UsageCardKind.ClaudeSession, changedKind);
        Assert.Equal(OverlayLayoutOptions.FiveHourCardHeight + 10, changedHeight);
        Assert.Contains(window.GetVisualDescendants().OfType<Border>(), border => border.Height == changedHeight);
        window.Close();
    }, CancellationToken.None);

    [Fact]
    public Task SettingsIncludeFontControlsAndExistingAppearanceControls() => Session.Dispatch(() =>
    {
        var window = new SettingsWindow(new CompanionSettings(), UiText.For(UiLanguage.English));
        window.Show();
        Assert.Single(window.GetVisualDescendants().OfType<Slider>());
        Assert.Contains(window.GetVisualDescendants().OfType<TextBox>(), box => box.Text == string.Empty);
        window.Close();
    }, CancellationToken.None);
}
