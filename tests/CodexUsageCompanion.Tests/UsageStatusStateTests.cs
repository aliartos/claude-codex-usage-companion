using CodexUsageCompanion.Configuration;
using CodexUsageCompanion.Lifecycle;
using CodexUsageCompanion.Ui;
using Xunit;

namespace CodexUsageCompanion.Tests;

public sealed class UsageStatusStateTests
{
    private const string ClaudeError = "Claude session expired. Run 'claude' to refresh your session.";

    [Fact]
    public void HidingClaudeBarsPreservesErrorAcrossCodexRefresh()
    {
        var status = new UsageStatusState();
        status.Update(null, ClaudeError, UsageProvider.Claude);
        status.ApplyProviders(new CompanionSettings { ShowClaudeSession = false, ShowClaudeWeekly = false });
        status.Update(DateTimeOffset.UtcNow, null, UsageProvider.Codex);
        Assert.Equal(ClaudeError, status.Error);
    }

    [Fact]
    public void DisablingClaudeClearsErrorAndIgnoresLateResults()
    {
        var status = new UsageStatusState();
        var updatedAt = DateTimeOffset.UtcNow;
        status.Update(updatedAt, null, UsageProvider.Codex);
        status.Update(null, ClaudeError, UsageProvider.Claude);
        status.ApplyProviders(new CompanionSettings { EnableClaudeUsage = false });
        status.Update(null, ClaudeError, UsageProvider.Claude);
        Assert.Null(status.Error);
        Assert.Equal(updatedAt, status.UpdatedAt);
    }

    [Fact]
    public void ClaudeSuccessClearsOnlyClaudeError()
    {
        var status = new UsageStatusState();
        status.Update(null, ClaudeError, UsageProvider.Claude);
        status.Update(null, "Codex unavailable", UsageProvider.Codex);
        status.Update(DateTimeOffset.UtcNow, null, UsageProvider.Claude);
        Assert.Equal("Codex unavailable", status.Error);
        status.Update(DateTimeOffset.UtcNow, null, UsageProvider.Codex);
        Assert.Null(status.Error);
    }
}
