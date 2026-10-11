using CodexUsageCompanion.Configuration;
using CodexUsageCompanion.Lifecycle;

namespace CodexUsageCompanion.Ui;

internal sealed class UsageStatusState
{
    private readonly Dictionary<UsageProvider, string> _errors = [];
    private bool _claudeEnabled = true;
    private bool _codexEnabled = true;
    private string? _generalError;

    public DateTimeOffset? UpdatedAt { get; private set; }
    public string? Error => _generalError ?? _errors.GetValueOrDefault(UsageProvider.Claude)
        ?? _errors.GetValueOrDefault(UsageProvider.Codex);

    public void ApplyProviders(CompanionSettings settings)
    {
        _claudeEnabled = settings.EnableClaudeUsage;
        _codexEnabled = settings.EnableCodexUsage;
        if (!_claudeEnabled)
            _errors.Remove(UsageProvider.Claude);
        if (!_codexEnabled)
            _errors.Remove(UsageProvider.Codex);
    }

    public void Update(DateTimeOffset? updatedAt, string? error, UsageProvider? provider = null)
    {
        if (provider is UsageProvider.Claude && !_claudeEnabled ||
            provider is UsageProvider.Codex && !_codexEnabled)
            return;

        _generalError = provider is null ? error : null;
        if (provider is { } source)
        {
            if (string.IsNullOrWhiteSpace(error))
                _errors.Remove(source);
            else
                _errors[source] = error;
        }
        if (updatedAt is { } timestamp)
            UpdatedAt = timestamp;
    }
}
